export function getAuthHeaders(): HeadersInit {
  const token = sessionStorage.getItem('token')

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  return headers
}

export function handleUnauthorized(response: Response): boolean {
  if (response.status === 401) {
    sessionStorage.removeItem('token')
    return true
  }

  return false
}