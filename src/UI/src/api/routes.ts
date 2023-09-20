import { http } from "@/utils/http";

type Result = {
  succeeded: boolean;
  data: Array<any>;
};

export const getAsyncRoutes = () => {
  //return http.request<Result>("get", "http://localhost:8848/getAsyncRoutes");
  return http.request<Result>("get", "/api/Auth/LoginerPermissionRouter");
};
