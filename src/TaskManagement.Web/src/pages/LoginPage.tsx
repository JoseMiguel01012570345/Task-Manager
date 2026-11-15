import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../api/session";

export function LoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("dev-user");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await login(username.trim(), password);
      void navigate("/");
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "No se pudo entrar.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section>
      <h1>Entrar</h1>
      {error && <p className="error">{error}</p>}
      <form className="stack" onSubmit={(event) => void onSubmit(event)}>
        <label>
          Usuario
          <input value={username} onChange={(event) => setUsername(event.target.value)} required autoComplete="username" />
        </label>
        <label>
          Contraseña
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
            autoComplete="current-password"
          />
        </label>
        <button type="submit" disabled={submitting}>
          Entrar
        </button>
      </form>
    </section>
  );
}
