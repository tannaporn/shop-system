"use server";

import { revalidatePath } from "next/cache";
import { createServerApiClient } from "@/lib/api/server";

export async function createProduct(data: {
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
}) {
  const api = await createServerApiClient();
  await api.post("/api/products", data);
  revalidatePath("/admin/products");
}

export async function updateProduct(
  id: string,
  data: { name: string; description: string | null; price: number }
) {
  const api = await createServerApiClient();
  await api.put(`/api/products/${id}`, data);
  revalidatePath("/admin/products");
}

export async function deleteProduct(id: string) {
  const api = await createServerApiClient();
  await api.delete(`/api/products/${id}`);
  revalidatePath("/admin/products");
}

export async function updateStock(id: string, stockQuantity: number) {
  const api = await createServerApiClient();
  await api.put(`/api/products/${id}/stock`, { stockQuantity });
  revalidatePath("/admin/products");
}
