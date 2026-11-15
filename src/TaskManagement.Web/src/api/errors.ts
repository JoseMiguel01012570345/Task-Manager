import { ApiError } from "./client";

export function describeHttpError(error: unknown): string {
  if (!(error instanceof ApiError)) {
    return "No se pudo completar la operación.";
  }

  switch (error.status) {
    case 401:
      return "La sesión expiró. Vuelve a entrar.";
    case 403:
      return "No tienes permiso para esta operación.";
    case 404:
      return "La tarea no está disponible.";
    case 409:
      return "La tarea cambió. Vuelve a cargarla antes de guardar.";
    case 422:
    case 400:
      return "Los datos no son válidos.";
    case 429:
      return "Demasiadas solicitudes. Espera un momento y vuelve a intentarlo.";
    default:
      return error.status >= 500
        ? "El servicio no pudo completar la operación."
        : "No se pudo completar la operación.";
  }
}
