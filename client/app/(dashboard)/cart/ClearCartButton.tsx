"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { apiClient } from "@/lib/api/client";

export function ClearCartButton() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  async function handleClear() {
    if (!confirm("ลบสินค้าทั้งหมดในตะกร้า?")) return;
    setLoading(true);
    try {
      await apiClient.delete("/api/cart/items");
      router.refresh();
    } catch {
      setLoading(false);
    }
  }

  return (
    <button
      onClick={handleClear}
      disabled={loading}
      className="text-sm text-red-500 hover:text-red-700 disabled:opacity-50"
    >
      {loading ? "กำลังลบ..." : "ล้างตะกร้า"}
    </button>
  );
}
