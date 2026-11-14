export type UpcomingReservation = {
  reservationId: number
  serviceId: number
  serviceProviderId: number
  clientId: number
  locationId: number
  date: string
  time: string
  duration: number
  clientName: string
  serviceName: string
  serviceProviderName: string
  locationName: string
  roomName: string
  roomId: number
  request: boolean
  partOfPackage: boolean
  note: string | null
  status: string
}

export type ResLog = {
  logId: number
  tenantId: number
  reservationId: number
  updatedByStaffId: number
  logTime: string
  logInfo: string
  logFieldNumber: number
  logValueBefore: string
  logValueAfter: string
}
