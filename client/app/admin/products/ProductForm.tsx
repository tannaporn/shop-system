"use client";

import { useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { createProduct, updateProduct } from "@/features/admin/products/actions";
import { Button } from "@/components/ui/Button";
import type { Product } from "@/types/product";

type Props = {
  product?: Product;
};

export function ProductForm({ product }: Props) {
  const router = useRouter();
  const [name, setName] = useState(product?.name ?? "");
  const [description, setDescription] = useState(product?.description ?? "");
  const [price, setPrice] = useState(product?.price !== undefined ? String(product.price) : "");
  const [stock, setStock] = useState(product?.stockQuantity !== undefined ? String(product.stockQuantity) : "");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const parsedPrice = parseFloat(price);
    const parsedStock = parseInt(stock, 10);
    if (isNaN(parsedPrice) || parsedPrice < 0) {
      setError("กรุณากรอกราคาที่ถูกต้อง");
      setLoading(false);
      return;
    }
    if (!product && (isNaN(parsedStock) || parsedStock < 0)) {
      setError("กรุณากรอกจำนวนสต็อกที่ถูกต้อง");
      setLoading(false);
      return;
    }

    try {
      if (product) {
        await updateProduct(product.id, {
          name,
          description: description || null,
          price: parsedPrice,
        });
      } else {
        await createProduct({
          name,
          description: description || null,
          price: parsedPrice,
          stockQuantity: parsedStock,
        });
      }
      router.push("/admin/products");
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "เกิดข้อผิดพลาด");
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-w-lg">
      <div className="space-y-1">
        <label htmlFor="name" className="text-sm font-medium">
          ชื่อสินค้า <span className="text-red-500">*</span>
        </label>
        <input
          id="name"
          required
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full rounded-lg border border-border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </div>

      <div className="space-y-1">
        <label htmlFor="description" className="text-sm font-medium">
          รายละเอียด
        </label>
        <textarea
          id="description"
          rows={3}
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full rounded-lg border border-border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary resize-none"
        />
      </div>

      <div className="space-y-1">
        <label htmlFor="price" className="text-sm font-medium">
          ราคา (บาท) <span className="text-red-500">*</span>
        </label>
        <input
          id="price"
          type="number"
          min={0}
          step="0.01"
          required
          value={price}
          onChange={(e) => setPrice(e.target.value)}
          className="w-full rounded-lg border border-border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </div>

      {!product && (
        <div className="space-y-1">
          <label htmlFor="stock" className="text-sm font-medium">
            จำนวนสต็อกเริ่มต้น <span className="text-red-500">*</span>
          </label>
          <input
            id="stock"
            type="number"
            min={0}
            required
            value={stock}
            onChange={(e) => setStock(e.target.value)}
            className="w-full rounded-lg border border-border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>
      )}

      {error && <p className="text-sm text-red-500">{error}</p>}

      <div className="flex gap-3">
        <Button type="submit" loading={loading}>
          {product ? "บันทึกการแก้ไข" : "เพิ่มสินค้า"}
        </Button>
        <Button
          type="button"
          variant="outline"
          disabled={loading}
          onClick={() => router.back()}
        >
          ยกเลิก
        </Button>
      </div>
    </form>
  );
}
