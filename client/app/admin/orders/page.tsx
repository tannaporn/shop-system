import { createServerApiClient } from "@/lib/api/server";
import type { Order, OrderStatus } from "@/types/order";
import type { ApiResponse } from "@/types/index";
import { Badge } from "@/components/ui/Badge";
import { OrderActions } from "./OrderActions";

export const dynamic = "force-dynamic";

const STATUS_OPTIONS: { label: string; value: string }[] = [
  { label: "ทั้งหมด", value: "" },
  { label: "Pending", value: "Pending" },
  { label: "Confirmed", value: "Confirmed" },
  { label: "Completed", value: "Completed" },
  { label: "Cancelled", value: "Cancelled" },
];

async function getOrders(status?: string): Promise<Order[]> {
  const api = await createServerApiClient();
  const res = await api.get<ApiResponse<Order[]>>("/api/orders", {
    params: status ? { status } : undefined,
  });
  return res.data.data;
}

export default async function AdminOrdersPage({
  searchParams,
}: {
  searchParams: { status?: string };
}) {
  const activeStatus = searchParams.status ?? "";
  let orders: Order[] = [];
  let error: string | null = null;

  try {
    orders = await getOrders(activeStatus || undefined);
  } catch (e) {
    error = e instanceof Error ? e.message : "เกิดข้อผิดพลาด";
  }

  return (
    <div className="space-y-6">
      <h2 className="text-xl font-bold">คำสั่งซื้อทั้งหมด</h2>

      {/* Status filter tabs */}
      <div className="flex gap-2 flex-wrap">
        {STATUS_OPTIONS.map(({ label, value }) => (
          <a
            key={value}
            href={value ? `/admin/orders?status=${value}` : "/admin/orders"}
            className={[
              "rounded-full px-3 py-1 text-sm font-medium transition-colors",
              activeStatus === value
                ? "bg-primary text-primary-foreground"
                : "bg-muted/30 text-muted hover:bg-muted/60",
            ].join(" ")}
          >
            {label}
          </a>
        ))}
      </div>

      {error && <p className="text-sm text-red-500">{error}</p>}

      <div className="overflow-x-auto rounded-lg border border-border">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="px-4 py-3 text-left font-medium">Order ID</th>
              <th className="px-4 py-3 text-left font-medium">ลูกค้า</th>
              <th className="px-4 py-3 text-right font-medium">ยอดรวม</th>
              <th className="px-4 py-3 text-center font-medium">สถานะ</th>
              <th className="px-4 py-3 text-left font-medium">วันที่</th>
              <th className="px-4 py-3 text-right font-medium">จัดการ</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {orders.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-muted">
                  ไม่พบคำสั่งซื้อ
                </td>
              </tr>
            )}
            {orders.map((order) => (
              <tr key={order.id} className="hover:bg-muted/20">
                <td className="px-4 py-3 font-mono text-xs text-muted">
                  #{order.id.slice(-8).toUpperCase()}
                </td>
                <td className="px-4 py-3">{order.customerName ?? "-"}</td>
                <td className="px-4 py-3 text-right">฿{order.totalPrice.toLocaleString()}</td>
                <td className="px-4 py-3 text-center">
                  <Badge variant={order.status} />
                </td>
                <td className="px-4 py-3 text-muted">
                  {new Date(order.createdAt).toLocaleDateString("th-TH")}
                </td>
                <td className="px-4 py-3 text-right">
                  <OrderActions orderId={order.id} status={order.status as OrderStatus} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
