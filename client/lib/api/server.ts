import "server-only";
import axios from "axios";
import { createSupabaseServerClient } from "@/lib/auth/supabase-server";

export async function createServerApiClient() {
  const supabase = await createSupabaseServerClient();
  const { data: { session } } = await supabase.auth.getSession();

  const instance = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL,
    headers: {
      "Content-Type": "application/json",
      ...(session?.access_token && {
        Authorization: `Bearer ${session.access_token}`,
      }),
    },
  });

  // แปลง axios error ให้อ่านง่าย
  instance.interceptors.response.use(
    (res) => res,
    (error) => {
      const message =
        error.response?.data?.error?.message ?? error.response?.data?.message ?? error.message ?? "Something went wrong";
      return Promise.reject(new Error(message));
    }
  );

  return instance;
}
