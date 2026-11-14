export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  tokenString: string;
};

export async function login(request: LoginRequest): 
 Promise<LoginResponse> {
  const response = await fetch("https://localhost:7021/api/Auth/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error("Login failed");
  }

  return response.json();
}