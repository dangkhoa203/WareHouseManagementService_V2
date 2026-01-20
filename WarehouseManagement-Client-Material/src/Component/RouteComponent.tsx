import {Route, Routes} from "react-router";
import HomePage from "./HomePage/HomePage.tsx";
import Intro from "./HomePage/Intro.tsx";
import AboutUs from "./HomePage/AboutUs.tsx";
import Function from "./HomePage/Function.tsx";
import Login from "./HomePage/Login.tsx";
import Register from "./HomePage/Register.tsx";
import WorkSpace from "./WorkPage/WorkSpace.tsx";
import NhomKhachHang from "./WorkPage/NhomKhachHang/NhomKhachHang.tsx";
import userInfoFunction from "../Type/userInfoFunction.tsx";
import TaoNhomKhachHang from "./WorkPage/NhomKhachHang/TaoNhomKhachHang.tsx";
import SuaNhomKhachHang from "./WorkPage/NhomKhachHang/SuaNhomKhachHang.tsx";

export default function RouteComponent(){
    return (
        <Routes location={location} key={location.pathname}>
            <Route path="/"  element={<HomePage />}>
                <Route path="" element={<Intro/> }></Route>
                <Route path="About" element={<AboutUs/> }></Route>
                <Route path="Function" element={<Function/> }></Route>
                <Route path="Login" element={<Login/> }></Route>
                <Route path="Register" element={<Register/> }></Route>
            </Route>
            <Route path="/Workspace" element={ <WorkSpace/>}>
                <Route path="NhomKhachHang" >
                    <Route index element={<NhomKhachHang/> }></Route>
                    <Route path="Tao"  element={<TaoNhomKhachHang/>}></Route>
                    <Route path="Sua/:id"  element={<SuaNhomKhachHang/>}></Route>
                </Route>
            </Route>
        </Routes>
    )
}