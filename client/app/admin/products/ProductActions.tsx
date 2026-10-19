"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { deleteProduct, updateStock } from "@/features/admin/products/actions";
import { Button } from "@/components/ui/Button";
import type { Product } from "@/types/product";

export function ProductActions({ product }: { product: Product }) {
  const router = useRouter();
  const [stockInput, setStockInput] = useState(String(product.stockQuantity));
  const [showStockModal, setShowStockModal] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleDelete() {
    setLoading(true);
    setError(null);
    try {
      await deleteProduct(product.id);
      setShowDeleteConfirm(false);
      router.refresh();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "เกิดข้อผิดพลาด");
    } finally {
      setLoading(false);
    }
  }

  async function handleUpdateStock() {
    const qty = parseInt(stockInput, 10);
    if (isNaN(qty) || qty < 0) {
      setError("กรุณากรอกจำนวนที่ถูกต้อง (>= 0)");
      return;
    }
    setLoading(true);
    setError(null);
    try {
      await updateStock(product.id, qty);
      setShowStockModal(false);
      router.refresh();
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "เกิดข้อผิดพลาด");
    } finally {
      setLoading(false);
    }
  }

  return (
    <>
      <div className="inline-flex gap-2">
        <Link
          href={`/admin/products/${product.id}/edit`}
          className="rounded px-2 py-1 text-xs font-medium border border-border hover:bg-muted/20 transition-colors"
        >
          แก้ไข
        </Link>
        <button
          onClick={() => { setStockInput(String(product.stockQuantity)); setShowStockModal(true); setError(null); }}
          className="rounded px-2 py-1 text-xs font-medium border border-border hover:bg-muted/20 transition-colors"
        >
          สต็อก
        </button>
        <button
          onClick={() => { setShowDeleteConfirm(true); setError(null); }}
          className="rounded px-2 py-1 text-xs font-medium text-red-600 border border-red-200 hover:bg-red-50 transition-colors"
        >
          ลบ
        </button>
      </div>

      {showDeleteConfirm && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="w-full max-w-sm rounded-xl bg-white p-6 shadow-lg space-y-4">
            <h3 className="font-semibold text-lg">ยืนยันการลบสินค้า</h3>
            <p className="text-sm text-muted">
              ต้องการลบ <strong>{product.name}</strong> ใช่หรือไม่? ไม่สามารถลบได้ถ้ามี order ที่ยังค้างอยู่
            </p>
            {error && <p className="text-sm text-red-500">{error}</p>}
            <div className="flex gap-3">
              <Button variant="danger" loading={loading} onClick={handleDelete}>
                ลบสินค้า
              </Button>
              <Button
                variant="outline"
                disabled={loading}
                onClick={() => setShowDeleteConfirm(false)}
              >
                ยกเลิก
              </Button>
            </div>
          </div>
        </div>
      )}

      {showStockModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="w-full max-w-sm rounded-xl bg-white p-6 shadow-lg space-y-4">
            <h3 className="font-semibold text-lg">อัปเดตสต็อก — {product.name}</h3>
            <input
              type="number"
              min={0}
              value={stockInput}
              onChange={(e) => setStockInput(e.target.value)}
              className="w-full rounded-lg border border-border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
            />
            {error && <p className="text-sm text-red-500">{error}</p>}
            <div className="flex gap-3">
              <Button loading={loading} onClick={handleUpdateStock}>
                บันทึก
              </Button>
              <Button
                variant="outline"
                disabled={loading}
                onClick={() => setShowStockModal(false)}
              >
                ยกเลิก
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
