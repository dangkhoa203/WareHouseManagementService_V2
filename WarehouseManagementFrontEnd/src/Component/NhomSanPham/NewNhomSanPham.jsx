import { useForm } from "react-hook-form";
import {useState} from "react";
import {Link, Navigate, useNavigate} from "react-router-dom";
export default function NewNhomSanPham(props){
    const [error, setError] = useState("")
    const [loading, setLoading] = useState(false)
    const { register, handleSubmit, watch, formState: { errors } } = useForm();
    const navigate = useNavigate();
    const onSubmit = data => tao(data);
    const checkData = (data) => {
        setError("");
        if (data.name.length === 0) {
            setError("Bạn chưa nhập đủ thông tin!")
            return false;
        }
        return true;
    }
    const tao=async (data)=>{
        if(checkData(data)) {
            try {
                setLoading(true)
                const response = await fetch('https://localhost:7075/api/v1/Product-groups', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    credentials: 'include',
                    body: JSON.stringify(data)
                })
                if (!response.ok) {
                    const text = await response.text();
                    setError(text)
                    setLoading(false)
                    throw Error(text);
                }
                setLoading(false)
                navigate("/NhomSanPham")
            } catch (e) {
                setLoading(false)
            }
        }
    }
    if(!props.user.isLogged && props.user.userId===""){
        return <Navigate to="/login"></Navigate>
    }
    return(
        <>
            <div className="container d-flex justify-content-center p-5 ">
                <form className="w-50 border border-3 rounded-4 p-3 bg-white d-flex justify-content-center flex-column"
                      onSubmit={handleSubmit(onSubmit)}>
                    <p className="h1 text-center mb-3">Tạo nhóm sản phẩm</p>
                    <button type={"button"} className="btn btn-outline-dark border-3 fw-bold  text-start mb-2"
                            style={{width: "120px"}}
                            onClick={() => navigate(-1)}><i className="bi bi-backspace"> Quay về</i></button>
                    <div className="d-flex flex-column">
                        <div className="form-floating mb-3">
                            <input type="text" className="form-control rounded-0 border-3"
                                   id="floatingInput" {...register("name")} placeholder="Tên"/>
                            <label htmlFor="floatingInput">Tên</label>
                        </div>
                        <div className="form-floating">
                            <textarea className="form-control rounded-0 border-3 h-25"
                                      id="floatingInput" {...register("description")} placeholder="Mô tả"/>
                            <label htmlFor="floatingInput">Mô tả</label>
                        </div>
                    </div>
                    <h5 className="text-danger text-center">{error}</h5>
                    <div className="d-flex flex-column justify-content-center w-100">
                        <button
                            className={`btn btn-outline-success fw-bolder border-3 w-50 m-auto mt-2 rounded-5 ${loading ? "disabled" : ""}`}
                            type="submit">{loading ?
                            <>
                                <span className="spinner-grow spinner-grow-sm ms-1" role="status"
                                      aria-hidden="true"></span>
                                <span className="spinner-grow spinner-grow-sm ms-1" role="status"
                                      aria-hidden="true"></span>
                                <span className="spinner-grow spinner-grow-sm ms-1" role="status"
                                      aria-hidden="true"></span>
                            </> : <>
                                Tạo
                            </>
                        }</button>
                    </div>
                </form>
            </div>
        </>
    )
}