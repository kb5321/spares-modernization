import { useEffect, useState } from 'react'
import { login } from './api/authApi'
import { getAuthHeaders, handleUnauthorized } from './api/authHeaders'
import './App.css'

type Room = {
    roomId: number
    roomName: string
    locationId: number
}

type Client = {
  clientId: number
  firstName: string
  lastName: string
}

type Service = {
  serviceId: number
  description: string
  amount: number
  serviceDuration: number
  catId: number
}

type Location = {
  locationId: number
  name: string
}

type Staff = {
  staffId: number
  firstName: string
  lastName: string
}

type UpcomingReservation = {
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
  roomId : number
  request: boolean
  partOfPackage: boolean
  note: string | null
  status: string
}

type ResLog = {
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

function App() {

  const [rooms, setRooms] = useState<Room[]>([])

  const [loginEmail, setLoginEmail] = useState('')
  const [loginPassword, setLoginPassword] = useState('')
  const [loginMessage, setLoginMessage] = useState('')

  const [reservationLogs, setReservationLogs] = useState<ResLog[]>([])
  const [historyReservationId, setHistoryReservationId] =
  useState<number | null>(null);
  const [isHistoryLoading, setIsHistoryLoading] =
  useState(false);


  const [upcomingReservations, setUpcomingReservations] =
  useState<UpcomingReservation[]>([])

  const [inactiveReservations, setInactiveReservations] =
  useState<UpcomingReservation[]>([]);

  const [rescheduleAvailableTimes, setRescheduleAvailableTimes] =
  useState<string[]>([]);

  const [clients, setClients] = useState<Client[]>([])
  const [selectedClientId, setSelectedClientId] = useState('')

  const [services, setServices] = useState<Service[]>([])
  const [selectedServiceId, setSelectedServiceId] = useState('')

 const [locations, setLocations] = useState<Location[]>([])
 const [selectedLocationId, setSelectedLocationId] = useState('')

 const [providers, setProviders] = useState<Staff[]>([])
 const [selectedProviderId, setSelectedProviderId] = useState('')

 const [selectedDate, setSelectedDate] = useState('')

 const [availableTimes, setAvailableTimes] = useState<string[]>([])
 const [selectedTime, setSelectedTime] = useState('')

 const [bookingMessage, setBookingMessage] = useState('')

 const [availabilityRefresh, setAvailabilityRefresh] = useState(0)

 const [isBooking, setIsBooking] = useState(false)

 const [upcomingRefresh, setUpcomingRefresh] = useState(0)

 const [reschedulingReservation, setReschedulingReservation] =
  useState<UpcomingReservation | null>(null);

  const [rescheduleDate, setRescheduleDate] = useState("");
  const [rescheduleTime, setRescheduleTime] = useState("");

  const [rescheduleAvailabilityMessage, setRescheduleAvailabilityMessage] =
  useState('');

  const [isAuthenticated, setIsAuthenticated] = useState(
  () => sessionStorage.getItem('token') !== null
)

const [editingReservation, setEditingReservation] =
    useState<UpcomingReservation | null>(null);

  const [staff, setStaff] = useState<Staff[]>([]);




  /* Load the appointments  */
  useEffect(() => {
    fetch('https://localhost:7021/api/Reservation/upcoming')
      .then((response) => {

         if (handleUnauthorizedResponse(response)) {
            throw new Error('Your session has expired. Please log in again.')
          }
        if (!response.ok) {
          throw new Error('Failed to load upcoming reservations.')
        }

        return response.json()
      })
      .then((data) => {
        setUpcomingReservations(data)
      })
      .catch((error) => {
        console.error(error)
      })
  }, [upcomingRefresh])




  useEffect(() => {
    fetch('https://localhost:7021/Clients')
      .then((response) => response.json())
      .then((data) => setClients(data))

    
  }, [])

  useEffect(() => {
  fetch('https://localhost:7021/api/Service')
    .then((response) => response.json())
    .then((data) => setServices(data))
}, [])

useEffect(() => {
  fetch('https://localhost:7021/api/Location')
    .then((response) => response.json())
    .then((data) => setLocations(data))
}, [])

useEffect(() => {
  if (!selectedServiceId || !selectedLocationId) {
    setProviders([])
    setSelectedProviderId('')
    return
  }

  fetch(
    `https://localhost:7021/api/Staff/providers?serviceId=${selectedServiceId}&locationId=${selectedLocationId}`
  )
    .then((response) => response.json())
    .then((data) => {
      setProviders(data)
      setSelectedProviderId('')
    })
}, [selectedServiceId, selectedLocationId])

useEffect(() => {
  if (
    !selectedServiceId ||
    !selectedLocationId ||
    !selectedProviderId ||
    !selectedDate
  ) {
    setAvailableTimes([])
    setSelectedTime('')
    return
  }

  fetch(
    `https://localhost:7021/api/Booking/available-times?staffId=${selectedProviderId}&locationId=${selectedLocationId}&serviceId=${selectedServiceId}&date=${selectedDate}`
  )
    .then((response) => response.json())
    .then((data) => {
      setAvailableTimes(data)
      setSelectedTime('')
    })
}, [
  selectedServiceId,
  selectedLocationId,
  selectedProviderId,
  selectedDate,
  availabilityRefresh,
])

  /* Load active staff */
  useEffect(() => {
      loadStaff();
  }, []);

    /* Load rooms */
  useEffect(() => {
      loadRooms();
  }, []);

  function createReservation() {
    if (
      !selectedClientId ||
      !selectedServiceId ||
      !selectedLocationId ||
      !selectedProviderId ||
      !selectedDate ||
      !selectedTime
    ) {
      setBookingMessage('Please complete all reservation fields.')
      return
    }

    setIsBooking(true)
    setBookingMessage('Creating reservation...')
   
    

    fetch('https://localhost:7021/api/Reservation', {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify({
        tenantId: 1,
        locationId: Number(selectedLocationId),
        date: selectedDate,
        time: selectedTime,

        // Server derives the real duration from Service.
        duration: 0,

        serviceProviderId: Number(selectedProviderId),

        // Server forces new reservations to Pending.
        reservationStatus: 0,

        note: '',
        serviceId: Number(selectedServiceId),
        clientId: Number(selectedClientId),

        // Temporary until authenticated staff exists.
        bookedById: 2,

        // Server automatically assigns the room.
        roomId: 0,

        request: false,
        partOfPackage: false,
        isRepeatClient: false,
      }),
    })
      .then(async (response) => {
         if (handleUnauthorizedResponse(response)) {
            throw new Error('Your session has expired. Please log in again.')
          }
        if (!response.ok) {
          const message = await response.text()
          throw new Error(message || 'Unable to create reservation.')
        }

        return response.json()
      })
      .then((reservation) => {
        setBookingMessage(
          `Reservation #${reservation.reservationId} created successfully.`
        )

        setSelectedTime('')
        setAvailabilityRefresh((value) => value + 1)
        setUpcomingRefresh((value) => value + 1)

      })
      .catch((error) => {
        setBookingMessage(`Reservation failed: ${error.message}`)
      })
      .finally(() => { setIsBooking(false)   })
  }

    const updateReservationStatus = (
    reservationId: number,
    status: number
  ) => {
    fetch(
          `https://localhost:7021/api/Booking/${reservationId}/status?status=${status}`,
          {
            method: 'PATCH',
            headers: getAuthHeaders()
          })
      .then((response) => {
        
        
          if (handleUnauthorizedResponse(response)) {
            throw new Error('Your session has expired. Please log in again.')
          }

          if (!response.ok) {
            throw new Error('Failed to update reservation status.')
          }

          return response.json()

      })
      .then(() => {
        setUpcomingReservations((currentReservations) =>
          currentReservations.map((reservation) =>
            reservation.reservationId === reservationId
              ? {
                  ...reservation,
                  status: status === 1 ? 'Cancelled' : 'Void'
                }
              : reservation
          )
        )
      })
      .catch((error) => {
        console.error(error)
      })
}
function handleRescheduleClick(reservation: UpcomingReservation) {
  setReschedulingReservation(reservation);
  setRescheduleDate(reservation.date);

   loadRescheduleAvailableTimes(
    reservation,
    reservation.date
  );
}



const handleEditReservation = (reservation: UpcomingReservation) => {
    setEditingReservation(reservation);
 
};

async function loadRescheduleAvailableTimes(
  reservation: UpcomingReservation,
  date: string
) {
  const response = await fetch(
    `https://localhost:7021/api/Booking/available-times?serviceId=${reservation.serviceId}&locationId=${reservation.locationId}&staffId=${reservation.serviceProviderId}&date=${date}`
  );

   if (handleUnauthorizedResponse(response)) {
            throw new Error('Your session has expired. Please log in again.')
    }

  if (!response.ok) {
    setRescheduleAvailableTimes([]);
    setRescheduleAvailabilityMessage('Unable to load available times.');
    return;
  }

  const times = await response.json();
  setRescheduleAvailableTimes(times);

  if (times.length === 0) {
  setRescheduleAvailabilityMessage('No available time.');
  } else {
    setRescheduleAvailabilityMessage('');
  }
}

async function handleSaveReschedule() {
  if (!reschedulingReservation || !rescheduleDate || !rescheduleTime) {
    return;
  }

  const response = await fetch(
    `https://localhost:7021/api/Reservation/${reschedulingReservation.reservationId}/reschedule`,
    {
      method: "PATCH",
      headers: getAuthHeaders(),
      body: JSON.stringify({
        date: rescheduleDate,
        time: rescheduleTime,
      }),
    }
  );
   if (handleUnauthorizedResponse(response)) {
            throw new Error('Your session has expired. Please log in again.')
    }

  if (!response.ok) {
    console.error("Failed to reschedule reservation");
    return;
  }

  setReschedulingReservation(null);
  setRescheduleDate("");
  setRescheduleTime("");
  setRescheduleAvailableTimes([]);

  setUpcomingRefresh((value) => value + 1);
}

async function handleViewHistory(reservationId: number) {
  setHistoryReservationId(reservationId);
  setReservationLogs([]);
  setIsHistoryLoading(true);

  try {
    const response = await fetch(
      `https://localhost:7021/api/Reservation/${reservationId}/logs`,
      {
        headers: getAuthHeaders()
      }
    );

    if (handleUnauthorizedResponse(response)) {
      throw new Error(
        'Your session has expired. Please log in again.'
      );
    }

    if (!response.ok) {
      setReservationLogs([]);
      return;
    }

    const logs: ResLog[] = await response.json();
    setReservationLogs(logs);
  } catch (error) {
    console.error('Failed to load reservation history:', error);
    setReservationLogs([]);
  } finally {
    setIsHistoryLoading(false);
  }
}


async function handleLogin() {
  try {
    const result = await login({
      email: loginEmail,
      password: loginPassword,
    })

    sessionStorage.setItem('token', result.tokenString)
    setIsAuthenticated(true)
    setLoginMessage('Login successful')

  } catch {
    setLoginMessage('Login failed')
  }
}

function handleLogout() {
  sessionStorage.removeItem('token')
  setIsAuthenticated(false)
  setLoginMessage('Logged out')
  setLoginEmail("")
  setLoginPassword("")
}

function handleUnauthorizedResponse(response: Response): boolean {
  if (!handleUnauthorized(response)) {
    return false
  }

  setIsAuthenticated(false)
  setLoginMessage('Your session has expired. Please log in again.')

  return true
}

 const loadInactiveReservations = async () => {
    console.log("SICK RESERVATION FUNCTION CALLED");

    const response = await fetch("https://localhost:7021/api/Reservation/inactive");

    console.log("Response:", response);

    if (!response.ok) {
        console.error("Failed to load Cancelled/Void reservations");
        return;
    }

    const data = await response.json();

    console.log("Sick reservations:", data);
    setInactiveReservations(data);
};

const saveReservationDate = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),
     
            body: JSON.stringify({
                date: editingReservation.date
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();
         console.error("Failed to update reservation date:", error);
         setBookingMessage(error)
        return;
    }

    //console.log("RESERVATION DATE UPDATED");
    setBookingMessage('RESERVATION DATE UPDATED')
};

  const saveReservationTime = async () => {
      if (!editingReservation) {
          return;
      }

      const response = await fetch(
          `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
          {
              method: "PATCH",
              headers: getAuthHeaders(),
             
                body: JSON.stringify({
                  time: editingReservation.time.length === 5
                      ? `${editingReservation.time}:00`
                      : editingReservation.time
              })
          }
      );

      if (!response.ok) {
          const error = await response.text();

          console.error("Failed to update reservation time:", error);
          setBookingMessage(error);

          return;
      }

      setBookingMessage("RESERVATION TIME UPDATED");
  };

  const saveReservationServiceProvider = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                serviceProviderId:
                    editingReservation.serviceProviderId
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error(
            "Failed to update service provider:",
            error
        );

        setBookingMessage(error);
        return;
    }

    setBookingMessage("SERVICE PROVIDER UPDATED");
};

const saveReservationClient = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                clientId: editingReservation.clientId
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update reservation client:", error);
        setBookingMessage(error);
        return;
    }

    setBookingMessage("RESERVATION CLIENT UPDATED");
};

const loadRooms = async () => {
    const response = await fetch(
        "https://localhost:7021/api/Room",
        {
            headers: getAuthHeaders()
        }
    );

    if (!response.ok) {
        const error = await response.text();
        console.error("Failed to load rooms:", error);
        return;
    }

    const data: Room[] = await response.json();

    setRooms(data);
};

  const loadStaff = async () => {
    const response = await fetch(
        "https://localhost:7021/api/Staff",
        {
            headers: getAuthHeaders()
        }
    );

    if (!response.ok) {
        const error = await response.text();
        console.error("Failed to load staff:", error);
        return;
    }

    const data: Staff[] = await response.json();

    setStaff(data);
};
const saveReservationService = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                serviceId: editingReservation.serviceId
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update reservation service:", error);
        setBookingMessage(error);

        return;
    }

    setBookingMessage("RESERVATION SERVICE UPDATED");
};

const saveReservationStatus = async () => {
    if (!editingReservation) {
        return;
    }

    const statusValue =
        editingReservation.status === "Pending" ? 0 :
        editingReservation.status === "Cancelled" ? 1 :
        editingReservation.status === "Void" ? 2 :
        3;

    const response = await fetch(
        `https://localhost:7021/api/Booking/${editingReservation.reservationId}/status?status=${statusValue}`,
        {
            method: "PATCH",
            headers: getAuthHeaders()
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error(
            "Failed to update reservation status:",
            error
        );

        setBookingMessage(error);
        return;
    }

    setBookingMessage("RESERVATION STATUS UPDATED");
};
const saveReservationNote = async () => {
   
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                note: editingReservation.note
            })
        }
    );
   

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update reservation note:", error);
        setBookingMessage(error);
        return;
    }

    setBookingMessage("RESERVATION NOTE UPDATED");
};

const saveReservationRoom = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                roomId: editingReservation.roomId
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update reservation room:", error);
        setBookingMessage(error);
        return;
    }

    setBookingMessage("RESERVATION ROOM UPDATED");
};

const saveReservationRequest = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                request: editingReservation.request
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update request status:", error);
        setBookingMessage(error);
        return;
    }

    setBookingMessage("REQUEST STATUS UPDATED");
};

const saveReservationPartOfPackage = async () => {
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                partOfPackage: editingReservation.partOfPackage
            })
        }
    );

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update part of package status:", error);
        setBookingMessage(error);
        return;
    }

    setBookingMessage("PART OF PACKAGE STATUS UPDATED");
};

const saveReservationDuration = async () => {
   
    if (!editingReservation) {
        return;
    }

    const response = await fetch(
        `https://localhost:7021/api/Reservation/${editingReservation.reservationId}`,
        {
            method: "PATCH",
            headers: getAuthHeaders(),

            body: JSON.stringify({
                duration: editingReservation.duration
            })
        }
    );
   

    if (!response.ok) {
        const error = await response.text();

        console.error("Failed to update reservation duration:", error);
        setBookingMessage(error);
        return;
    }

    setBookingMessage("RESERVATION DURATION UPDATED");
};

const editingService = services.find(
    (service) => service.serviceId === editingReservation?.serviceId
);

const isMassage = editingService?.catId === 1;

  return (
    <>

    <section>
    
        <h2>Staff Login</h2>
        {!isAuthenticated ? (
          <>

        <input
          type="email"
          placeholder="Email"
          value={loginEmail}
          onChange={(e) => setLoginEmail(e.target.value)}
        />

        <input
          type="password"
          placeholder="Password"
          value={loginPassword}
          onChange={(e) => setLoginPassword(e.target.value)}
        />

        <button onClick={handleLogin}>
          Login
        </button>
        </>) : (<button onClick={handleLogout}>
        Logout
      </button>)}

        <p>{loginMessage}</p>
      
    </section>


   {isAuthenticated && (
      <>
    
      <header>
        <h1>SpaRes</h1>
        <p>Reservation Management System</p>
      </header>

      <main>
            
        <button
          type="button"
          onClick={() => {
              if (inactiveReservations.length > 0) {
                  setInactiveReservations([]);
              } else {
                  loadInactiveReservations();
              }
          }}
      >
          {inactiveReservations.length > 0
              ? "Hide Cancelled / Void"
              : "Show Cancelled / Void"}
      </button>

    {inactiveReservations.length > 0 && (
    <div className="inactive-reservations">
        <h2>Cancelled / Void Reservations</h2>

        {inactiveReservations.map((reservation) => (
            <div
                className="inactive-reservation"
                key={reservation.reservationId}
            >
                <div>
                    <strong>{reservation.clientName}</strong>
                    {" — "}
                    {reservation.serviceName}
                </div>

                <div>
                    {reservation.date} {reservation.time}
                    {" — "}
                    {reservation.serviceProviderName}
                </div>

                <strong>{reservation.status}</strong>
            </div>
        ))}
    </div>
)}

{editingReservation && (
  <div className="edit-reservation">
        <h2>Edit Reservation</h2>

        {/*
                          =============================================
                            EDIT RESERVATION — CLIENT
                          =============================================

        */}

        <div>
            <label>
                <strong>Client:</strong>{" "}

                <select
                    value={editingReservation.clientId}
                    onChange={(e) =>
                        setEditingReservation({
                            ...editingReservation,
                            clientId: Number(e.target.value)
                        })
                    }
                >
                    {clients.map((client) => (
                        <option
                            key={client.clientId}
                            value={client.clientId}
                        >
                            {client.firstName} {client.lastName}
                        </option>
                    ))}
                </select>

                <button
                    type="button"
                    onClick={saveReservationClient}
                >
                    Save Client
                </button>
            </label>
        </div>

        {/* 
                          =============================================
                              EDIT RESERVATION — SERVICE
                          ============================================= 
        */}

        <div>
            <label>
                <strong>Service:</strong>{" "}

                <select
                    value={editingReservation.serviceId}
                    onChange={(e) =>
                        setEditingReservation({
                            ...editingReservation,
                            serviceId: Number(e.target.value)
                        })
                    }
                >
                    {services.map((service) => (
                        <option
                            key={service.serviceId}
                            value={service.serviceId}
                        >
                            {service.description}
                        </option>
                    ))}
                </select>

                <button
                    type="button"
                    onClick={saveReservationService}
                >
                    Save Service
                </button>
            </label>
        </div>

      {/* 
                        =============================================
                            EDIT RESERVATION — SERVICE PROVIDER
                        =============================================

      */}

        <div>
            <label>
                <strong>Service Provider:</strong>{" "}

                <select
                    value={editingReservation.serviceProviderId}
                    onChange={(e) =>
                        setEditingReservation({
                            ...editingReservation,
                            serviceProviderId: Number(e.target.value)
                        })
                    }
                >
                    {staff.map((staffMember) => (
                        <option
                            key={staffMember.staffId}
                            value={staffMember.staffId}
                        >
                            {staffMember.firstName} {staffMember.lastName}
                        </option>
                    ))}
                </select>
            </label>

            <button
                type="button"
                onClick={saveReservationServiceProvider}
            >
                Save Provider
            </button>
        </div>


        {/* 
                      =============================================
                          EDIT RESERVATION — DATE
                      ============================================= 

        */}

        <div>
            <label>
                <strong>Date:</strong>{" "}

                <input
                    type="date"
                    value={editingReservation.date}
                    onChange={(e) =>
                        setEditingReservation({
                            ...editingReservation,
                            date: e.target.value
                        })
                    }
                />
            </label>

            <button
                type="button"
                onClick={saveReservationDate}
            >
                Save Date
            </button>
        </div>

        {/* 
                      =============================================
                          EDIT RESERVATION — TIME
                      ============================================= 

        */}

        <div>
        <label>
            <strong>Time:</strong>{" "}

            {/* TEMPORARY hour list.
                Later this will come from Location Open/Close hours. */}
            <select
                value={editingReservation.time.substring(0, 2)}
                onChange={(e) => {
                    const minute =
                        editingReservation.time.substring(3, 5);

                    setEditingReservation({
                        ...editingReservation,
                        time: `${e.target.value}:${minute}`
                    });
                }}
            >
                {Array.from({ length: 15 }, (_, i) => {
                    const hour = i + 8;
                    const value = hour.toString().padStart(2, "0");

                    return (
                        <option key={value} value={value}>
                            {hour > 12
                                ? `${hour - 12} PM`
                                : hour === 12
                                ? "12 PM"
                                : `${hour} AM`}
                        </option>
                    );
                })}
            </select>

            {" : "}

            <select
                value={editingReservation.time.substring(3, 5)}
                onChange={(e) => {
                    const hour =
                        editingReservation.time.substring(0, 2);

                    setEditingReservation({
                        ...editingReservation,
                        time: `${hour}:${e.target.value}`
                    });
                }}
            >
                <option value="00">00</option>
                <option value="15">15</option>
                <option value="30">30</option>
                <option value="45">45</option>
            </select>

            {" "}

            <button
                type="button"
                onClick={saveReservationTime}
            >
                Save Time
            </button>
        </label>

        <div>
            Selected time: {editingReservation.time}
        </div>
    </div>

    {/* 
              =============================================
                  EDIT RESERVATION — NOTE
              ============================================= 
    */}

    <div>
        <label>
            <strong>Note:</strong>{" "}

            <textarea
                value={editingReservation.note ?? ""}
                onChange={(e) =>
                    setEditingReservation({
                        ...editingReservation,
                        note: e.target.value
                    })
                }
                rows={3}
            />

            {" "}

            <button
                type="button"
                onClick={saveReservationNote}
            >
                Save Note
            </button>
        </label>
      </div>


    {/* 
                =============================================
                    EDIT RESERVATION — ROOM
                ============================================= 
    
    */}

    <div>
        <label>
            <strong>Room:</strong>{" "}

            <select
                value={editingReservation.roomId}
                onChange={(e) =>
                    setEditingReservation({
                        ...editingReservation,
                        roomId: Number(e.target.value)
                    })
                }
            >
                {rooms
                    .filter(
                        (room) =>
                            room.locationId === editingReservation.locationId
                    )
                    .map((room) => (
                        <option
                            key={room.roomId}
                            value={room.roomId}
                        >
                            {room.roomName}
                        </option>
                    ))}
            </select>

            <button
                type="button"
                onClick={saveReservationRoom}
            >
                Save Room
            </button>
        </label>
    </div>



    {/* 
                      =============================================
                          EDIT RESERVATION — REQUESTED PROVIDER
                      ============================================= 
    */}

    <div>
        <label>
            <strong>Requested Provider:</strong>{" "}

            <input
                type="checkbox"
                checked={editingReservation.request}
                onChange={(e) =>
                    setEditingReservation({
                        ...editingReservation,
                        request: e.target.checked
                    })
                }
            />

            <button
                type="button"
                onClick={saveReservationRequest}
            >
                Save Request
            </button>
        </label>
    </div>


    {/* 
                =============================================
                    EDIT RESERVATION — PART OF PACKAGE
                =============================================
   */}

    <div>
        <label>
            <strong>Part of Package:</strong>{" "}

            <input
                type="checkbox"
                checked={editingReservation.partOfPackage}
                onChange={(e) =>
                    setEditingReservation({
                        ...editingReservation,
                        partOfPackage: e.target.checked
                    })
                }
            />

            <button
                type="button"
                onClick={saveReservationPartOfPackage}
            >
                Save PartOfPackage
            </button>
        </label>
    </div>

    {/* 
                  =============================================
                      EDIT RESERVATION — DURATION
                  ============================================= 
    
    */}

    <div>
        <label>
            <strong>Duration:</strong>{" "}

            <input
                type="number"
                value={editingReservation.duration}
                disabled={isMassage}
                onChange={(e) =>
                    setEditingReservation({
                        ...editingReservation,
                        duration: Number(e.target.value)
                    })
                }
            />
        </label>

        <button
            type="button"
            onClick={saveReservationDuration}
            disabled={isMassage}
        >
            Save Duration
        </button>
    </div>


    {/* 
                  =============================================
                      EDIT RESERVATION — STATUS
                  ============================================= 
    */}

    <div>
        <label>
            <strong>Status:</strong>{" "}

            <select
                value={
                    editingReservation.status === "Pending" ? 0 :
                    editingReservation.status === "Cancelled" ? 1 :
                    editingReservation.status === "Void" ? 2 :
                    3
                }
                onChange={(e) => {
                    const value = Number(e.target.value);

                    const statusName =
                        value === 0 ? "Pending" :
                        value === 1 ? "Cancelled" :
                        value === 2 ? "Void" :
                        "Confirmed";

                    setEditingReservation({
                        ...editingReservation,
                        status: statusName
                    });
                }}
            >
                <option value={0}>Pending</option>
                <option value={1}>Cancelled</option>
                <option value={2}>Void</option>
                <option value={3}>Confirmed</option>
            </select>
        </label>

        <button
          type="button"
          onClick={saveReservationStatus}
        >
            Save Status
        </button>
    </div>
                  {/* ===== END RESERVATION EDIT     ======*/}

  </div>
)}


{/* 
                  =====================================================
                                NEW RESERVATION
                  ===================================================== 
*/}

  <h2>New Reservation</h2>
  {/* 
        =============================================
            NEW RESERVATION — CLIENT
        =============================================
  */}

<label htmlFor="client">Client</label>

<select
    id="client"
    value={selectedClientId}
    onChange={(event) => setSelectedClientId(event.target.value)}
>
    <option value="">Select a client</option>

    {clients.map((client) => (
        <option key={client.clientId} value={client.clientId}>
            {client.firstName} {client.lastName}
        </option>
    ))}
</select>

  <label htmlFor="client">Client</label>

  <select
    id="client"
    value={selectedClientId}
    onChange={(event) => setSelectedClientId(event.target.value)}
  >
    <option value="">Select a client</option>

    {clients.map((client) => (
      <option key={client.clientId} value={client.clientId}>
        {client.firstName} {client.lastName}
      </option>
    ))}
  </select>
   {/* 
        =============================================
            NEW RESERVATION — SERVICE
        =============================================
  */}

    <label htmlFor="service">Service</label>

  <select
    id="service"
    value={selectedServiceId}
    onChange={(event) => setSelectedServiceId(event.target.value)}
  >
    <option value="">Select a service</option>

    {services.map((service) => (
      <option key={service.serviceId} value={service.serviceId}>
        {service.description} {service.serviceDuration}
      </option>
    ))}
  </select>
   {/* 
        =============================================
            NEW RESERVATION — LOCATION
        =============================================
  */}
  <label htmlFor="location">Location</label>

  <select
    id="location"
    value={selectedLocationId}
    onChange={(event) => setSelectedLocationId(event.target.value)}
  >
    <option value="">Select a location</option>

    {locations.map((location) => (
      <option key={location.locationId} value={location.locationId}>
        {location.name}
      
      </option>
    ))}
  </select>

   {/* 
        =============================================
            NEW RESERVATION — PROVIDER
        =============================================
  */}

  <label htmlFor="provider">Provider</label>

  <select
    id="provider"
    value={selectedProviderId}
    onChange={(event) => setSelectedProviderId(event.target.value)}
    disabled={!selectedServiceId || !selectedLocationId}
  >
    <option value="">
      {selectedServiceId && selectedLocationId && providers.length === 0
        ? 'No providers available'
        : 'Select a provider'}
    </option>

    {providers.map((provider) => (
      <option key={provider.staffId} value={provider.staffId}>
        {provider.firstName} {provider.lastName}
      </option>
    ))}
  </select>

   {/* 
        =============================================
            NEW RESERVATION — DATE
        =============================================
  */}

    <label htmlFor="date">Date</label>

        <input
          id="date"
          type="date"
          min={new Date().toLocaleDateString('en-CA')}
          value={selectedDate}
          onChange={(event) => setSelectedDate(event.target.value)}
        />
   
    {/* 
        =============================================
            NEW RESERVATION — TIME
        =============================================
  */}
  <label htmlFor="time">Time</label>
    <select
      id="time"
      value={selectedTime}
      
      onChange={(e) => {
        setSelectedTime(e.target.value)
        setBookingMessage('')
      }}

      disabled={
        !selectedServiceId ||
        !selectedLocationId ||
        !selectedProviderId ||
        !selectedDate
      }
    >
      <option value="">
        {selectedServiceId &&
        selectedLocationId &&
        selectedProviderId &&
        selectedDate &&
        availableTimes.length === 0
          ? 'No times available'
          : 'Select a time'}
      </option>

      {availableTimes.map((time) => (
        <option key={time} value={time}>
          {time.substring(0, 5)}
        </option>
      ))}
    </select>


    <button
        type="button"
        onClick={createReservation}
        disabled={
          isBooking ||
          !selectedClientId ||
          !selectedServiceId ||
          !selectedLocationId ||
          !selectedProviderId ||
          !selectedDate ||
          !selectedTime
        }

      >
{isBooking ? 'Creating...' : 'Create Reservation'}
  
  </button>

  <p>{bookingMessage}</p>

{/* =====================================================
          UPCOMING APPOINTMENTS
          =====================================================
*/}
    <h2>Upcoming Appointments</h2>

    <table>
      <thead>
        <tr>
          <th>Date</th>
          <th>Time</th>
          <th>Client</th>
          <th>Service</th>
          <th>Provider</th>
          <th>Location</th>
          <th>Room</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        {upcomingReservations.map((reservation) => (
          <tr key={reservation.reservationId}>
            <td>{reservation.date}</td>
            <td>{reservation.time.substring(0, 5)}</td>
            <td>{reservation.clientName}</td>
            <td>{reservation.serviceName}</td>
            <td>{reservation.serviceProviderName}</td>
            <td>{reservation.locationName}</td>
            <td>{reservation.roomName}</td>
            <td>{reservation.status}</td>
            <td>
        {reservation.status !== 'Cancelled' &&
          reservation.status !== 'Void' && (
            <>
              <button
                onClick={() =>
                  updateReservationStatus(
                    reservation.reservationId,
                    1
                  )
                }
              >
                Cancel
              </button>

              <button
                onClick={() =>
                  updateReservationStatus(
                    reservation.reservationId,
                    2
                  )
                }
              >
                Void
              </button>

              <button
                type="button"
                onClick={() => handleRescheduleClick(reservation)}
              >
                Reschedule
              </button>

              <button
                    type="button"
                    onClick={() => handleEditReservation(reservation)}
                >
                    Edit
                </button>
            </>
          )}

        <button
        type="button"
        onClick={() => handleViewHistory(reservation.reservationId)}
      >
        History
      </button>


      </td>
       
          </tr>
        ))}
      </tbody>
    </table>

{/* 
    =====================================================
        RESERVATION HISTORY
    ===================================================== 
*/}

    
    {historyReservationId !== null && (
  <div>
    <h3>Reservation History</h3>

    {isHistoryLoading ? (
        <p>Loading history...</p>
      ) : reservationLogs.length > 0 ? (
        reservationLogs.map((log) => (
          <div key={log.logId}>
            <p>
              {log.logInfo}: {log.logValueBefore} →{' '}
              {log.logValueAfter}
            </p>
          </div>
        ))
      ) : (
        <p>No history for this reservation.</p>
      )}



    <button
        type="button"
        onClick={() => {
          setHistoryReservationId(null)
          setReservationLogs([])
        }}
      >
      Close History
    </button>
  </div>
  
)}

{/* 
          =====================================================
              RESCHEDULE RESERVATION
          ===================================================== 
*/}

      {reschedulingReservation && (
  <div>
    <h3>Reschedule Reservation - 101</h3>

    <p>
      {reschedulingReservation.clientName} —{" "}
      {reschedulingReservation.serviceName}
    </p>

    <p>
      Current: {reschedulingReservation.date} at{" "}
      {reschedulingReservation.time.substring(0,5)}
    </p>
    

    <label>
      New Date:
      <input
        type="date"
        value={rescheduleDate}
        onChange={(e) => {
            const newDate = e.target.value;

            setRescheduleDate(newDate);
            setRescheduleTime('');

            if (reschedulingReservation && newDate) {
              loadRescheduleAvailableTimes(
                reschedulingReservation,
                newDate
              );
            }
          }}
      />
    </label>


    {rescheduleAvailabilityMessage && (
    <p>{rescheduleAvailabilityMessage}</p>
    )}
  
  {rescheduleAvailableTimes.length > 0 && (
  <div>
            <p>Available Times:</p>

            {rescheduleAvailableTimes.map((time) => (
            <button
              key={time}
              type="button"
              onClick={() => setRescheduleTime(time)}
            >
              {time.substring(0,5)}
            </button>
          ))}

          {rescheduleTime && (
          <p>Selected Time: {rescheduleTime}</p>
        )}

        {rescheduleTime && (
          <button
            type="button"
            onClick={handleSaveReschedule}
          >
            Save Reschedule
          </button>

          
        )}


        <div>
        <button
          type="button"
          onClick={() => {
            setReschedulingReservation(null)
            setRescheduleDate('')
            setRescheduleTime('')
            setRescheduleAvailableTimes([])
            setRescheduleAvailabilityMessage('')
          }}
        >
          Cancel Reschedule
        </button>
        </div>


      </div>
)}

  </div>
)}  
</main>
  </>
    )}


    </>
  )
}

export default App