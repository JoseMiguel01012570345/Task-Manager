import { setAccessToken } from "./client";

const storageKey = "taskmanagement.accessToken";

export function restoreSession(): void {
  setAccessToken(sessionStorage.getItem(storageKey));
}

export function saveSession(token: string): void {
  sessionStorage.setItem(storageKey, token);
  setAccessToken(token);
}

export function clearSession(): void {
  sessionStorage.removeItem(storageKey);
  setAccessToken(null);
}

export function hasSession(): boolean {
  return sessionStorage.getItem(storageKey) !== null;
}

export async function login(username: string, password: string): Promise<void> {
  const body = new URLSearchParams({
    client_id: "taskmanagement-web",
    grant_type: "password",
    username,
    password,
  });
  const response = await fetch("/realms/taskmanagement/protocol/openid-connect/token", {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded", Accept: "application/json" },
    body,
  });
  if (!response.ok) {
    throw new Error("Usuario o contraseña no válidos.");
  }
  const payload = (await response.json()) as { access_token?: string };
  if (!payload.access_token) {
    throw new Error("Usuario o contraseña no válidos.");
  }
  saveSession(payload.access_token);
}
