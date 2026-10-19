"use client";

import { useState } from "react";
import { loginWithPassword, loginWithGoogle } from "../actions/login";
import { Button } from "@/components/ui/Button";
import type { LoginFormValues } from "../types";

export function LoginForm() {
  const [values, setValues] = useState<LoginFormValues>({
    email: "",
    password: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);

    const result = await loginWithPassword(values);

    if (!result.success) {
      setError(result.error);
      setLoading(false);
    }
  }

  async function handleGoogle() {
    setLoading(true);
    setError(null);
    const result = await loginWithGoogle();
    if (!result.success) {
      setError(result.error);
      setLoading(false);
    }
  }

  return (
    <div className="w-full max-w-sm bg-card rounded-2xl shadow-md px-8 py-10 space-y-6">
      {/* Header */}
      <div className="space-y-1 text-center">
        <h1 className="text-2xl font-bold">Login</h1>
        <p className="text-sm text-muted">
          Hey, Enter your details to sign in to your account
        </p>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="space-y-4">
        <input
          id="email"
          type="email"
          required
          autoComplete="email"
          value={values.email}
          onChange={(e) => setValues({ ...values, email: e.target.value })}
          className="w-full rounded-lg border border-border bg-white px-4 py-2.5 text-sm outline-none focus:ring-2 focus:ring-primary placeholder:text-muted"
          placeholder="Enter your email here"
        />

        <input
          id="password"
          type="password"
          required
          autoComplete="current-password"
          value={values.password}
          onChange={(e) => setValues({ ...values, password: e.target.value })}
          className="w-full rounded-lg border border-border bg-white px-4 py-2.5 text-sm outline-none focus:ring-2 focus:ring-primary placeholder:text-muted"
          placeholder="Enter your password here"
        />

        {error && (
          <p role="alert" className="text-sm text-red-500">
            {error}
          </p>
        )}

        <p className="text-center text-sm text-muted">
          Having trouble signing in?
        </p>

        <Button type="submit" loading={loading} size="lg">
          Login
        </Button>
      </form>

      {/* Divider */}
      <div className="relative">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-border" />
        </div>
        <div className="relative flex justify-center text-xs text-muted">
          <span className="bg-card px-2">— Or Sign in with —</span>
        </div>
      </div>

      {/* Google */}
      <Button
        type="button"
        variant="outline"
        size="lg"
        onClick={handleGoogle}
        loading={loading}
      >
        <svg viewBox="0 0 24 24" className="h-4 w-4 mr-2" aria-hidden="true">
          <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" />
          <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" />
          <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l3.66-2.84z" fill="#FBBC05" />
          <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" />
        </svg>
        Sign in with Google
      </Button>
    </div>
  );
}
