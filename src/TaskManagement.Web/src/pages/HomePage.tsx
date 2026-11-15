import { useState } from "react";
import { api, apiBaseUrl } from "../api/client";

export function HomePage() {
  const [status, setStatus] = useState("Sin comprobar");

  async function checkApi() {
    setStatus("Comprobando...");
    try {
      await api<unknown>("/api/v1/tasks?page=1&pageSize=1");
      setStatus("La API respondió.");
    } catch (error) {
      setStatus(error instanceof Error ? error.message : "No se pudo contactar la API.");
    }
  }

  return (
    <section>
      <h1>Laboratorio local</h1>
      <p>
        API configurada en <code>{apiBaseUrl()}</code>
      </p>
      <button type="button" onClick={() => void checkApi()}>
        Probar conexión
      </button>
      <p>{status}</p>
    </section>
  );
}
