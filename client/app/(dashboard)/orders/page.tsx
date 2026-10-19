import { createServerApiClient } from "@/lib/api/server";
import type { Order } from "@/types/order";
import type { ApiResponse } from "@/types/index";
import { Badge } from "@/components/ui/Badge";
import { EmptyState } from "@/components/ui/EmptyState";

export const dynamic = "force-dynamic";
import { CancelOrderButton } from "./CancelOrderButton";
import Link from "next/link";

export default async function OrdersPage() {
  let orders: Order[] = [];
  let error: string | null = null;

  try {
    const api = await createServerApiClient();
    const res = await api.get<ApiResponse<Order[]>>("/api/orders/my");
    orders = res.data.data;
  } catch (e) {
    error = e instanceof Error ? e.message : "เกิดข้อผิดพลาด";
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">ประวัติการสั่งซื้อ</h1>

      {error && <p className="text-sm text-red-500">{error}</p>}

      {!error && orders.length === 0 && (
        <EmptyState
          title="ยังไม่มีประวัติการสั่งซื้อ"
          description="สั่งซื้อสินค้าเพื่อดูประวัติที่นี่"
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

      {orders.length > 0 && (
        <div className="overflow-hidden rounded-xl border border-border bg-card">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border bg-gray-50">
                <th className="px-4 py-3 text-left font-medium">Order ID</th>
                <th className="px-4 py-3 text-left font-medium">วันที่</th>
                <th className="px-4 py-3 text-right font-medium">ยอดรวม</th>
                <th className="px-4 py-3 text-center font-medium">สถานะ</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
                <tr key={order.id} className="border-b border-border last:border-0">
                  <td className="px-4 py-3 font-mono text-xs text-muted">
                    #{order.id.slice(-8).toUpperCase()}
                  </td>
                  <td className="px-4 py-3">
                    {new Date(order.createdAt).toLocaleDateString("th-TH")}
                  </td>
                  <td className="px-4 py-3 text-right">
                    ฿{order.totalPrice.toLocaleString()}
                  </td>
                  <td className="px-4 py-3 text-center">
                    <Badge variant={order.status} />
                  </td>
                  <td className="px-4 py-3 text-right">
                    {order.status === "Pending" && (
                      <CancelOrderButton orderId={order.id} />
                    )}
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
