import Link from "next/link";
import { createServerApiClient } from "@/lib/api/server";
import type { Product } from "@/types/product";
import { ProductActions } from "./ProductActions";

export const dynamic = "force-dynamic";

async function getProducts(search?: string): Promise<Product[]> {
  const api = await createServerApiClient();
  const res = await api.get("/api/products", { params: search ? { search } : undefined });
  return res.data.data;
}

export default async function AdminProductsPage({
  searchParams,
}: {
  searchParams: { search?: string };
}) {
  const products = await getProducts(searchParams.search);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-bold">สินค้า</h2>
        <Link
          href="/admin/products/new"
          className="rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground hover:bg-primary-hover transition-colors"
        >
          เพิ่มสินค้า
        </Link>
      </div>

      <form method="get">
        <input
          name="search"
          defaultValue={searchParams.search}
          placeholder="ค้นหาสินค้า..."
          className="w-full max-w-sm rounded-lg border border-border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </form>

      <div className="overflow-x-auto rounded-lg border border-border">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="px-4 py-3 text-left font-medium">ชื่อสินค้า</th>
              <th className="px-4 py-3 text-right font-medium">ราคา</th>
              <th className="px-4 py-3 text-right font-medium">คลังสินค้า</th>
              <th className="px-4 py-3 text-right font-medium">จัดการ</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {products.length === 0 && (
              <tr>
                <td colSpan={4} className="px-4 py-8 text-center text-muted">
                  ไม่พบสินค้า
                </td>
              </tr>
            )}
            {products.map((product) => (
              <tr key={product.id} className="hover:bg-muted/20">
                <td className="px-4 py-3">
                  <p className="font-medium">{product.name}</p>
                  {product.description && (
                    <p className="text-xs text-muted line-clamp-1">{product.description}</p>
                  )}
                </td>
                <td className="px-4 py-3 text-right">฿{product.price.toLocaleString()}</td>
                <td className="px-4 py-3 text-right">
                  <span
                    className={
                      product.stockQuantity === 0
                        ? "text-red-500 font-medium"
                        : product.stockQuantity < 10
                        ? "text-yellow-600 font-medium"
                        : ""
                    }
                  >
                    {product.stockQuantity}
                  </span>
                </td>
                <td className="px-4 py-3 text-right">
                  <ProductActions product={product} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
