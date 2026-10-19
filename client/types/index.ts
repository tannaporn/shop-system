export type ApiResponse<T> = {
  data: T;
  message?: string;
};

export type PagedResult<T> = {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
};

export type ApiError = {
  message: string;
  statusCode: number;
};
