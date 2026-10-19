export type LoginFormValues = {
  email: string;
  password: string;
};

export type ActionResult =
  | { success: true }
  | { success: false; error: string };
