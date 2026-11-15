import { Link, Route, Routes } from "react-router-dom";
import { HomePage } from "./pages/HomePage";

export function App() {
  return (
    <>
      <header>
        <strong>Task Management</strong>
        <nav>
          <Link to="/">Inicio</Link>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<HomePage />} />
        </Routes>
      </main>
    </>
  );
}
