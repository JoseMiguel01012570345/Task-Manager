import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8088";
const suite = __ENV.SUITE === "1";

export const options = {
  vus: suite ? 3 : 10,
  duration: suite ? "8s" : "2m",
  gracefulStop: "1s",
  thresholds: {
    http_req_failed: ["rate<0.05"],
    http_req_duration: ["p(95)<1000"],
  },
};

export default function () {
  const response = http.get(`${base}/health/ready`);
  check(response, { "ready responds 200": (result) => result.status === 200 });
  sleep(0.5);
}
