import { createServerApiClient } from "@/lib/api/server";
import type { Cart } from "@/types/cart";
import type { ApiResponse } from "@/types/index";
import { EmptyState } from "@/components/ui/EmptyState";
import { CheckoutButton } from "./CheckoutButton";

export const dynamic = "force-dynamic";
import Link from "next/link";

export default async function CheckoutPage() {
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

  if (!error && items.length === 0) {
    return (
      <div className="max-w-2xl">
        <EmptyState
          title="ไม่มีสินค้าในตะกร้า"
          description="กรุณาเพิ่มสินค้าก่อนดำเนินการสั่งซื้อ"
          action={
            <Link
              href="/products"
              className="px-4 py-2 text-sm rounded-lg bg-primary text-primary-foreground hover:bg-primary-hover font-semibold"
            >
              ดูสินค้า
            </Link>
          }
        />
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-2xl">
      <h1 className="text-2xl font-bold">ยืนยันการสั่งซื้อ</h1>

      {error && <p className="text-sm text-red-500">{error}</p>}

      {items.length > 0 && (
        <>
          <div className="rounded-xl border border-border bg-card overflow-hidden">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-border bg-gray-50">
                  <th className="px-4 py-3 text-left font-medium">สินค้า</th>
                  <th className="px-4 py-3 text-center font-medium">จำนวน</th>
                  <th className="px-4 py-3 text-right font-medium">รวม</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.id} className="border-b border-border last:border-0">
                    <td className="px-4 py-3">{item.productName}</td>
                    <td className="px-4 py-3 text-center">{item.quantity}</td>
                    <td className="px-4 py-3 text-right">
                      ฿{item.subtotal.toLocaleString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex items-center justify-between rounded-xl border border-border bg-card px-6 py-4">
            <span className="font-semibold">ยอดรวมทั้งสิ้น</span>
            <span className="text-xl font-bold">
              ฿{(cart?.total ?? 0).toLocaleString()}
            </span>
          </div>

          <p className="text-sm text-muted">
            ชำระเงินแบบ Cash on Delivery (COD) — ชำระเมื่อรับสินค้า
          </p>

          <CheckoutButton />
        </>
      )}
    </div>
  );
}
