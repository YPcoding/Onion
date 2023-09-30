import { http } from "@/utils/http";

type Result = {
  succeeded: boolean;
  errors: Array<any>;
  errorMessage: string;
  code: number;
  data?: {
    /** 实体类 */
    entities?: Array<any>;
    /** 命名空间 */
    namespaceName?: string;
    /** 后端保存路径 */
    backendSavePath?: string;
    /** 前端保存路径 */
    frontendSavePath?: string;
  };
};

type ResultTable = {
  succeeded: boolean;
  errors: Array<any>;
  errorMessage: string;
  code: number;
  data?: {
    /** 总条目数 */
    totalItems?: number;
    /** 当前页数 */
    currentPage?: number;
    /** 每页大小 */
    pageSize?: number;
    /** 总页数 */
    totalPages?: number;
    /** 有上一页 */
    hasPreviousPage?: boolean;
    /** 有下一页 */
    hasNextPage?: boolean;
    /** 列表数据 */
    items: Array<any>;
  };
};

/** 查询 */
export const getGenerateCodeInfo = () => {
  return http.request<Result>("post", "/api/Code/Query/Save/Path");
};

/** 生成后端代码 */
export const generateBackendCode = (data?: object) => {
  return http.request<Result>("post", "/api/Code/Backend", {
    data
  });
};

/** 生成前端代码 */
export const generateFrontendCode = (data?: object) => {
  return http.request<Result>("post", "/api/Code/Frontend", {
    data
  });
};
