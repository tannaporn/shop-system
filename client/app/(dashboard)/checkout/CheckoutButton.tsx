"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { apiClient } from "@/lib/api/client";

export function CheckoutButton() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleCheckout() {
    setLoading(true);
    setError(null);
    try {
      await apiClient.post("/api/orders/checkout", {});
      router.push("/orders");
    } catch (e) {
      setError(e instanceof Error ? e.message : "เกิดข้อผิดพลาด");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex flex-col gap-2">
      {error && (
        <p className="text-sm text-red-500 bg-red-50 rounded-lg px-4 py-3">
          {error}
        </p>
      )}
      <div className="flex justify-end">
        <button
          onClick={handleCheckout}
          disabled={loading}
          className="px-6 py-3 text-base rounded-xl bg-primary text-primary-foreground hover:bg-primary-hover font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {loading ? "กำลังดำเนินการ..." : "ยืนยันการสั่งซื้อ"}
        </button>
      </div>
    </div>
  );
}
