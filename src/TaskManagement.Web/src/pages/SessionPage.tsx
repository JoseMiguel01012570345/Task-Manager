import { Link } from "react-router-dom";

export function SessionPage() {
  return (
    <section className="auth">
      <div className="panel auth-card">
        <p className="eyebrow">Sesión</p>
        <h1>Sesión expirada</h1>
        <p className="lede">La sesión ya no es válida. Vuelve a entrar para continuar.</p>
        <p>
          <Link className="button" to="/login">
            Entrar
          </Link>
        </p>
      </div>
    </section>
  );
}
