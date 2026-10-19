import { createServerApiClient } from "@/lib/api/server";
import type { Cart } from "@/types/cart";
import type { ApiResponse } from "@/types/index";
import { EmptyState } from "@/components/ui/EmptyState";
import { Button } from "@/components/ui/Button";
import { ClearCartButton } from "./ClearCartButton";
import { CartItemRow } from "./CartItemRow";
import Link from "next/link";

export const dynamic = "force-dynamic";

export default async function CartPage() {
  let cart: Cart | null = null;
  let error: string | null = null;

  try {
    const api = await createServerApiClient();
    const res = await api.get<ApiResponse<Cart>>("/api/cart");
    cart = res.data.data;
  } catch (e) {
    error = e instanceof Error ? e.message : "เกิดข้อผิดพลาด";
  }

  const items = cart?.items ?? [];

  return (
    <div className="space-y-6 max-w-2xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">ตะกร้าสินค้า</h1>
        {items.length > 0 && <ClearCartButton />}
      </div>

      {error && <p className="text-sm text-red-500">{error}</p>}

      {!error && items.length === 0 && (
        <EmptyState
          title="ตะกร้าว่างเปล่า"
          description="เพิ่มสินค้าเพื่อเริ่มต้นการสั่งซื้อ"
          action={
            <Link
              href="/products"
              className="px-4 py-2 text-sm rounded-lg bg-primary text-primary-foreground hover:bg-primary-hover font-semibold"
            >
              ดูสินค้า
            </Link>
          }
        />
      )}

      {items.length > 0 && (
        <>
          <div className="rounded-xl border border-border bg-card overflow-hidden">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-border bg-gray-50">
                  <th className="px-4 py-3 text-left font-medium">สินค้า</th>
                  <th className="px-4 py-3 text-right font-medium">ราคา/ชิ้น</th>
                  <th className="px-4 py-3 text-center font-medium">จำนวน</th>
                  <th className="px-4 py-3 text-right font-medium">รวม</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <CartItemRow key={item.id} item={item} />
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex items-center justify-between rounded-xl border border-border bg-card px-6 py-4">
            <span className="font-semibold">ยอดรวม</span>
            <span className="text-xl font-bold">
              ฿{(cart?.total ?? 0).toLocaleString()}
            </span>
          </div>

          <div className="flex justify-end">
            <Link href="/checkout">
              <Button size="lg" className="w-auto">
                ดำเนินการสั่งซื้อ
              </Button>
            </Link>
          </div>
        </>
      )}
    </div>
  );
}
