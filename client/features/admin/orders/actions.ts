"use server";

import { revalidatePath } from "next/cache";
import { createServerApiClient } from "@/lib/api/server";
import type { OrderStatus } from "@/types/order";

export async function updateOrderStatus(id: string, status: OrderStatus) {
  const api = await createServerApiClient();
  await api.put(`/api/orders/${id}/status`, { status });
  revalidatePath("/admin/orders");
}
