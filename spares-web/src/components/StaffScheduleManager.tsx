import { useEffect, useRef, useState } from 'react'

import type { Staff, Location } from '../types/referenceData'
import { getAuthHeaders } from '../api/authHeaders'


type StaffSchedule = {
    staffScheduleId: number
    staffId: number
    locationId: number
    date: string
    startTime: string
    endTime: string
}

type Props = {
    staff: Staff[]
    locations: Location[]
}

function StaffScheduleManager({ staff, locations }: Props) {
    const [selectedStaffId, setSelectedStaffId] = useState('')
    const [selectedLocationId, setSelectedLocationId] = useState('')
    const [selectedDate, setSelectedDate] = useState('')
    const [schedule, setSchedule] = useState<StaffSchedule | null>(null)
    const [scheduleMessage, setScheduleMessage] = useState('')
    const [isSaving, setIsSaving] = useState(false)
    const [isDeleting, setIsDeleting] = useState(false)
    const isSavingRef = useRef(false)
    const isDeletingRef = useRef(false)

    const [startTime, setStartTime] = useState('09:00')
    const [endTime, setEndTime] = useState('18:00')

    useEffect(() => {
        if (!selectedStaffId || !selectedLocationId || !selectedDate) {
            setSchedule(null)
            setScheduleMessage('')
            return
        }

        let cancelled = false

        const loadSchedule = async () => {
            setSchedule(null)
            setScheduleMessage('Loading schedule...')

            try {
                const response = await fetch(
                    'https://localhost:7021/api/StaffSchedule',
                    { headers: getAuthHeaders() }
                )

                if (!response.ok) {
                    throw new Error('Unable to load staff schedule.')
                }

                const schedules: StaffSchedule[] = await response.json()

                if (cancelled) return

                const matchingSchedule = schedules.find(
                    (item) =>
                        item.staffId === Number(selectedStaffId) &&
                        item.locationId === Number(selectedLocationId) &&
                        item.date === selectedDate
                )

                setSchedule(matchingSchedule ?? null)
                setStartTime(matchingSchedule?.startTime.substring(0, 5) ?? '09:00')
                setEndTime(matchingSchedule?.endTime.substring(0, 5) ?? '18:00')
                setScheduleMessage(
                    matchingSchedule ? '' : 'No schedule found for this date.'
                )
            } catch (error) {
                if (cancelled) return

                console.error(error)
                setSchedule(null)
                setScheduleMessage('Unable to load staff schedule.')
            }
        }

        loadSchedule()

        return () => {
            cancelled = true
        }
    }, [selectedStaffId, selectedLocationId, selectedDate])

    const saveSchedule = async () => {
        if (
            !selectedStaffId ||
            !selectedLocationId ||
            !selectedDate ||
            isSavingRef.current ||
            isDeletingRef.current
        ) {
            return
        }

        if (startTime >= endTime) {
            setScheduleMessage('End time must be after start time.')
            return
        }

        isSavingRef.current = true
        setIsSaving(true)

        try {
            const isEditing = schedule !== null
            const response = await fetch(
                isEditing
                    ? `https://localhost:7021/api/StaffSchedule/${schedule.staffScheduleId}`
                    : 'https://localhost:7021/api/StaffSchedule',
                {
                    method: isEditing ? 'PUT' : 'POST',
                    headers: getAuthHeaders(),
                    body: JSON.stringify({
                        staffId: Number(selectedStaffId),
                        locationId: Number(selectedLocationId),
                        date: selectedDate,
                        startTime: `${startTime}:00`,
                        endTime: `${endTime}:00`
                    })
                }
            )

            if (!response.ok) {
                throw new Error(await response.text())
            }

            const savedSchedule: StaffSchedule = isEditing
                ? {
                      ...schedule,
                      startTime: `${startTime}:00`,
                      endTime: `${endTime}:00`
                  }
                : await response.json()

            setSchedule(savedSchedule)
            setScheduleMessage(
                isEditing
                    ? 'Schedule updated successfully.'
                    : 'Schedule created successfully.'
            )
        } catch (error) {
            console.error('Schedule save error:', error)

            setScheduleMessage(
                error instanceof Error
                    ? error.message
                    : 'Failed to save schedule.'
            )
        } finally {
            isSavingRef.current = false
            setIsSaving(false)
        }
    }

    const deleteSchedule = async () => {
        if (!schedule || isSavingRef.current || isDeletingRef.current) {
            return
        }

        if (!window.confirm('Are you sure you want to delete this daily schedule?')) {
            return
        }

        isDeletingRef.current = true
        setIsDeleting(true)

        try {
            const response = await fetch(
                `https://localhost:7021/api/StaffSchedule/${schedule.staffScheduleId}`,
                {
                    method: 'DELETE',
                    headers: getAuthHeaders()
                }
            )

            if (!response.ok) {
                const errorMessage = await response.text()
                throw new Error(errorMessage || 'Failed to delete schedule.')
            }

            setSchedule(null)
            setStartTime('09:00')
            setEndTime('18:00')
            setScheduleMessage('Schedule deleted successfully.')
        } catch (error) {
            console.error('Schedule delete error:', error)

            setScheduleMessage(
                error instanceof Error
                    ? error.message
                    : 'Failed to delete schedule.'
            )
        } finally {
            isDeletingRef.current = false
            setIsDeleting(false)
        }
    }

    return (
        <section>
            <h2>Staff Scheduling Manager</h2>

            <label>Staff: </label>
            <select
                disabled={isSaving || isDeleting}
                value={selectedStaffId}
                onChange={(e) => setSelectedStaffId(e.target.value)}
            >
                <option value="">Select staff</option>
                {staff.map((member) => (
                    <option key={member.staffId} value={member.staffId}>
                        {member.firstName} {member.lastName}
                    </option>
                ))}
            </select>

            <label> Location: </label>
            <select
                disabled={isSaving || isDeleting}
                value={selectedLocationId}
                onChange={(e) => setSelectedLocationId(e.target.value)}
            >
                <option value="">Select location</option>
                {locations.map((location) => (
                    <option key={location.locationId} value={location.locationId}>
                        {location.name}
                    </option>
                ))}
            </select>

            <label> Date: </label>
            <input
                type="date"

                disabled={isSaving || isDeleting}
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
            />
            {scheduleMessage && <p>{scheduleMessage}</p>}

            {schedule && (
                <p>
                    <strong>Working Hours:</strong>{' '}
                    {schedule.startTime.substring(0, 5)}
                    {' — '}
                    {schedule.endTime.substring(0, 5)}
                </p>

            )}

            {selectedStaffId && selectedLocationId && selectedDate && (
                <div>
                    <h3>{schedule ? 'Edit Daily Schedule' : 'New Daily Schedule'}</h3>

                    <label htmlFor="startTime">Start Time: </label>
                    <input
                        id="startTime"
                        type="time"
                        disabled={isSaving || isDeleting}
                        value={startTime}
                        onChange={(e) => setStartTime(e.target.value)}
                    />

                    <label htmlFor="endTime"> End Time: </label>
                    <input
                        id="endTime"
                        type="time"
                        disabled={isSaving || isDeleting}
                        value={endTime}
                        onChange={(e) => setEndTime(e.target.value)}
                    />
                    <button
                        type="button"
                        disabled={isSaving || isDeleting}
                        onClick={saveSchedule}
                    >
                        {isSaving
                            ? 'Saving...'
                            : schedule
                              ? 'Update Schedule'
                              : 'Save Schedule'}
                    </button>
                    {schedule && (
                        <button
                            type="button"
                            disabled={isSaving || isDeleting}
                            onClick={deleteSchedule}
                        >
                            {isDeleting ? 'Deleting...' : 'Delete Schedule'}
                        </button>
                    )}
                </div>
            )}
        </section>
    )
}

export default StaffScheduleManager