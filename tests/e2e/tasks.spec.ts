import { expect, test } from "@playwright/test";

const base = process.env.BASE_URL || "http://localhost:5080";

test("create and read a task", async ({ request }) => {
  const created = await request.post(`${base}/api/v1/tasks`, {
    data: { title: "Flujo e2e", description: "Playwright", assigneeId: "dev-user" },
  });
  expect(created.status()).toBe(201);
  const body = (await created.json()) as { id: string; title: string };

  const fetched = await request.get(`${base}/api/v1/tasks/${body.id}`);
  expect(fetched.status()).toBe(200);
  expect(((await fetched.json()) as { title: string }).title).toBe("Flujo e2e");
});
