import RouteComponent from "./Component/RouteComponent.tsx";
import './CSS/NavBar.css'
import './CSS/Home.css'
import './CSS/Footer.css'
import {useEffect} from "react";
import {useUserInfo} from "./State/User.ts";
import {useSuspenseQuery} from "@tanstack/react-query";
import GetUserInfoQueryOption from "./Query/GetUserInfoQueryOption.ts";

export default function App(){

    const setUserInfo=useUserInfo((state)=> state.setUserInfo);
    const {data}=useSuspenseQuery(GetUserInfoQueryOption())
    useEffect(() => {
        if(data){
            setUserInfo(data)
        }
    }, [data]);

    return (
        <div style={{minHeight: "100vh",padding:"0",minWidth:"100wh"}}>
            <RouteComponent  />
        </div>
    )
}