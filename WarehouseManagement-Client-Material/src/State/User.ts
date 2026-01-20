import {create} from "zustand"
import {userInfo} from "../Type/userInfo.tsx";

export type userInfoState={
    user:userInfo,
    setUserInfo:(userInfo:userInfo)=> void,
    clearUserInfo:()=>void,
}
export const useUserInfo = create<userInfoState>((set)=>({
    user:{
        userName: 'default',
        userFullName: '',
        userEmail: '',
        userId: '',
        isLoggedIn: false,
    },
    setUserInfo:(userInfo:userInfo)=>(
        set({user:userInfo})
    ),
    clearUserInfo:()=>{
        set({user:
                {
                    userName: '',
                    userFullName: '',
                    userEmail: '',
                    userId: '',
                    isLoggedIn: false,
                }
            }
        )
    }
}))