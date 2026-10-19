import { notFound } from "next/navigation";
import { createServerApiClient } from "@/lib/api/server";
import type { Product } from "@/types/product";
import { ProductForm } from "../../ProductForm";

export const dynamic = "force-dynamic";

async function getProduct(id: string): Promise<Product | null> {
  try {
    const api = await createServerApiClient();
    const res = await api.get(`/api/products`);
    const products: Product[] = res.data.data;
    return products.find((p) => p.id === id) ?? null;
  } catch {
    return null;
  }
}

export default async function EditProductPage({
  params,
}: {
  params: { id: string };
}) {
  const product = await getProduct(params.id);
  if (!product) notFound();

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-bold">แก้ไขสินค้า — {product.name}</h2>
      <ProductForm product={product} />
    </div>
  );
}
