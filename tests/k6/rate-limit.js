import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8088";
const suite = __ENV.SUITE === "1";

export const options = {
  vus: suite ? 8 : 20,
  duration: suite ? "8s" : "20s",
  gracefulStop: "1s",
};

export default function () {
  const response = http.get(`${base}/api/v1/tasks`);
  check(response, {
    "limited or served": (result) => result.status === 200 || result.status === 429 || result.status === 401,
  });
  if (response.status === 429) {
    check(response, { "retry after": (result) => result.headers["Retry-After"] !== undefined });
  }
  sleep(0.05);
}
