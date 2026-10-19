import { ProductForm } from "../ProductForm";

export default function NewProductPage() {
  return (
    <div className="space-y-6">
      <h2 className="text-xl font-bold">เพิ่มสินค้าใหม่</h2>
      <ProductForm />
    </div>
  );
}
