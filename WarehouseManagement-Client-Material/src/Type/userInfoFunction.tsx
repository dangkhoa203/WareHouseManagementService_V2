import {userInfo} from "./userInfo.tsx";

export type userInfoFunction={
    user:userInfo,
    getInfo:()=> Promise<void>
}