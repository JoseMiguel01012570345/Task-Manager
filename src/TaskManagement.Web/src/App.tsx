import { useEffect } from "react";
import { Link, NavLink, Route, Routes, useLocation, useNavigate } from "react-router-dom";
import { setUnauthorizedHandler } from "./api/client";
import { clearSession, hasSession } from "./api/session";
import { LoginPage } from "./pages/LoginPage";
import { SessionPage } from "./pages/SessionPage";
import { TaskDetailPage } from "./pages/TaskDetailPage";
import { TaskFormPage } from "./pages/TaskFormPage";
import { TaskListPage } from "./pages/TaskListPage";

function SessionBridge() {
  const navigate = useNavigate();
  useEffect(() => {
    setUnauthorizedHandler(() => {
      clearSession();
      navigate("/login");
    });
    return () => setUnauthorizedHandler(null);
  }, [navigate]);
  return null;
}

function Header() {
  const navigate = useNavigate();
  return (
    <header className="topbar">
      <Link to="/" className="brand">
        <span className="brand-mark" aria-hidden="true" />
        Task Management
      </Link>
      <nav className="topnav">
        {hasSession() ? (
          <>
            <NavLink to="/tasks/new" className={({ isActive }) => (isActive ? "nav-link active" : "nav-link")}>
              Nueva
            </NavLink>
            <button
              className="nav-button"
              type="button"
              onClick={() => {
                clearSession();
                navigate("/login");
              }}
            >
              Salir
            </button>
          </>
        ) : (
          <NavLink to="/login" className={({ isActive }) => (isActive ? "nav-link active" : "nav-link")}>
            Entrar
          </NavLink>
        )}
      </nav>
    </header>
  );
}

export function App() {
  const location = useLocation();
  return (
    <>
      <SessionBridge />
      <Header />
      <main className="shell">
        <div key={location.pathname} className="stage">
        <Routes location={location}>
          <Route path="/" element={<TaskListPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/session" element={<SessionPage />} />
          <Route path="/tasks/new" element={<TaskFormPage />} />
          <Route path="/tasks/:id" element={<TaskDetailPage />} />
          <Route path="/tasks/:id/edit" element={<TaskFormPage />} />
        </Routes>
        </div>
      </main>
    </>
  );
}
