import { createServerApiClient } from "@/lib/api/server";
import type { Product } from "@/types/product";
import type { ApiResponse } from "@/types/index";
import { Badge } from "@/components/ui/Badge";
import { EmptyState } from "@/components/ui/EmptyState";

export const dynamic = "force-dynamic";
import { AddToCartButton } from "./AddToCartButton";

type ProductsPageProps = {
  searchParams: Promise<{ search?: string }>;
};

export default async function ProductsPage({ searchParams }: ProductsPageProps) {
  const { search } = await searchParams;

  let products: Product[] = [];
  let error: string | null = null;

  try {
    const api = await createServerApiClient();
    const params = search ? { params: { search } } : {};
    const res = await api.get<ApiResponse<Product[]>>("/api/products", params);
    products = res.data.data;
  } catch (e) {
    error = e instanceof Error ? e.message : "เกิดข้อผิดพลาด";
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Products</h1>
      </div>

      {/* Search */}
      <form method="GET">
        <div className="flex gap-2 max-w-sm">
          <input
            name="search"
            defaultValue={search}
            placeholder="ค้นหาสินค้า..."
            className="flex-1 rounded-lg border border-border px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary"
          />
          <button
            type="submit"
            className="px-4 py-2 text-sm rounded-lg bg-primary text-primary-foreground hover:bg-primary-hover font-semibold"
          >
            ค้นหา
          </button>
        </div>
      </form>

      {error && (
        <p className="text-sm text-red-500">{error}</p>
      )}

      {!error && products.length === 0 && (
        <EmptyState
          title="ไม่พบสินค้า"
          description={search ? `ไม่พบสินค้าที่ตรงกับ "${search}"` : "ยังไม่มีสินค้าในระบบ"}
        />
      )}

      {products.length > 0 && (
        <div className="overflow-hidden rounded-xl border border-border bg-card">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border bg-gray-50">
                <th className="px-4 py-3 text-left font-medium">ชื่อสินค้า</th>
                <th className="px-4 py-3 text-left font-medium">คำอธิบาย</th>
                <th className="px-4 py-3 text-right font-medium">ราคา</th>
                <th className="px-4 py-3 text-right font-medium">Stock</th>
                <th className="px-4 py-3 text-center font-medium">สถานะ</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id} className="border-b border-border last:border-0">
                  <td className="px-4 py-3 font-medium">{product.name}</td>
                  <td className="px-4 py-3 text-muted max-w-xs truncate">
                    {product.description ?? "-"}
                  </td>
                  <td className="px-4 py-3 text-right">
                    ฿{product.price.toLocaleString()}
                  </td>
                  <td className="px-4 py-3 text-right">{product.stockQuantity}</td>
                  <td className="px-4 py-3 text-center">
                    {product.stockQuantity === 0 && (
                      <Badge variant="out-of-stock" />
                    )}
                  </td>
                  <td className="px-4 py-3 text-right">
                    <AddToCartButton
                      productId={product.id}
                      disabled={product.stockQuantity === 0}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
