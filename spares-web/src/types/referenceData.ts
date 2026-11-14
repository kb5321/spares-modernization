export type Room = {
  roomId: number
  roomName: string
  locationId: number
}

export type Client = {
  clientId: number
  firstName: string
  lastName: string
}

export type Service = {
  serviceId: number
  description: string
  amount: number
  serviceDuration: number
  catId: number
}

export type Location = {
  locationId: number
  name: string
}

export type Staff = {
  staffId: number
  firstName: string
  lastName: string
}
