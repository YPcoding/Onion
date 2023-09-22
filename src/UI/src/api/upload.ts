import { http } from "@/utils/http";
import { dataTool } from "echarts/core";

type Result = {
  succeeded: boolean;
  data?: string;
  errors: Array<string>;
  errorMessage: string;
  code: number;
};

/** 上传附件 */
export const uploadEnclosure = (data?: object) => {
  return http.request<Result>("post", "/api/Upload/ImageForBase64", {
    data
  });
};

/** 转base64 */
export const convertImageToBase64 = (data?: object) => {
  return http.request<Result>("post", "/api/Upload/ConvertImageToBase64", {
    data
  });
};
