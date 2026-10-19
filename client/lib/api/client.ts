"use client";
import axios from "axios";
import { createSupabaseBrowserClient } from "@/lib/auth/supabase-client";

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  headers: { "Content-Type": "application/json" },
});

// แนบ JWT token ทุก request อัตโนมัติ
apiClient.interceptors.request.use(async (config) => {
  const supabase = createSupabaseBrowserClient();
  const { data: { session } } = await supabase.auth.getSession();

  if (session?.access_token) {
    config.headers.Authorization = `Bearer ${session.access_token}`;
  }

  return config;
});

// แปลง error ให้อ่านง่าย
apiClient.interceptors.response.use(
  (res) => res,
  (error) => {
    const message =
      error.response?.data?.error?.message ?? error.response?.data?.message ?? error.message ?? "Something went wrong";
    return Promise.reject(new Error(message));
  }
);

export { apiClient };
