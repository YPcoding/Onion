import { T } from "@vueuse/motion/dist/nuxt-b4cb9b59";
import { RouteRecordName } from "vue-router";

export type cacheType = {
  mode: string;
  name?: RouteRecordName;
};

export type positionType = {
  startIndex?: number;
  length?: number;
};

export type appType = {
  sidebar: {
    opened: boolean;
    withoutAnimation: boolean;
    // 判断是否手动点击Collapse
    isClickCollapse: boolean;
  };
  layout: string;
  device: string;
};

export type multiType = {
  path: string;
  name: string;
  meta: any;
  query?: object;
  params?: object;
};

export type setType = {
  title: string;
  fixedHeader: boolean;
  hiddenSideBar: boolean;
};

export type userType = {
  username?: string;
  roles?: Array<string>;
  verifyCode?: string;
  currentPage?: number;
  userInfo?: {
    accessFailedCount: number;
    concurrencyStamp: string;
    created: string;
    email: string;
    id: string;
    isActive: boolean;
    isLive: boolean;
    lockoutEnabled: boolean;
    lockoutEnd: string;
    normalizedEmail: string;
    normalizedUserName: string;
    phoneNumberConfirmed: boolean;
    profilePictureDataUrl: string;
    twoFactorEnabled: boolean;
    userId: string;
    userName: string;
  };
};
