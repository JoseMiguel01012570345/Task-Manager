import { Link } from "react-router-dom";

export function SessionPage() {
  return (
    <section>
      <h1>Sesión expirada</h1>
      <p>La sesión ya no es válida. Vuelve a entrar para continuar.</p>
      <Link to="/">Ir al listado</Link>
    </section>
  );
}
