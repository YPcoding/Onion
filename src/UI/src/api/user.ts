import { http } from "@/utils/http";

export type UserResult = {
  succeeded: boolean;
  errors: Array<string>;
  errorMessage: string;
  code: number;
  data: {
    /** 用户名 */
    username: string;
    /** 当前登陆用户的角色 */
    roles: Array<string>;
    /** `token` */
    accessToken: string;
    /** 用于调用刷新`accessToken`的接口时所需的`token` */
    refreshToken: string;
    /** `accessToken`的过期时间（格式'xxxx/xx/xx xx:xx:xx'） */
    expires: Date;
  };
};

export type RefreshTokenResult = {
  succeeded: boolean;
  data: {
    /** `token` */
    accessToken: string;
    /** 用于调用刷新`accessToken`的接口时所需的`token` */
    refreshToken: string;
    /** `accessToken`的过期时间（格式'xxxx/xx/xx xx:xx:xx'） */
    expires: Date;
  };
};

/** 登录 */
export const getLogin = (data?: object) => {
  return http.request<UserResult>(
    "post",
    "/api/Auth/LoginByUserNameAndPassword",
    { data }
  );
};

/** 刷新token */
export const refreshTokenApi = (refreshToken: string) => {
  return http.request<RefreshTokenResult>(
    "post",
    `/api/Auth/RefreshToken/${refreshToken}`
  );
};
