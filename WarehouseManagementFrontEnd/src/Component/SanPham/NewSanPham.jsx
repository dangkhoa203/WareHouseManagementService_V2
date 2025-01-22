import { useForm } from "react-hook-form";
import {useEffect, useState} from "react";
import {Link, Navigate, useNavigate} from "react-router-dom";
import data from "bootstrap/js/src/dom/data.js";
export default function NewSanPham(props){
    const [dsGroup,setDsGroup]=useState([]);
    const [error, setError] = useState("")
    const [loading, setLoading] = useState(false)
    const { register, handleSubmit, watch, formState: { errors } } = useForm();
    const navigate = useNavigate();
    const onSubmit = data => tao(data);
    const getDs = async () =>{
        const response = await fetch('https://localhost:7075/api/v1/Product-Groups', {
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            method:"GET"
        });
        if (!response.ok) {
            const text = await response.text();
            throw Error(text);
        }
        const content = await response.json();
        setDsGroup(content);
    }
    useEffect(() => {
        getDs()
    }, []);
    const checkData=(data)=>{
        setError("")
        if(data.name.length===0||data.measureUnit.length===0||data.pricePerUnit.length===0){
            setError("Chưa nhập đầy đủ thông tin")
            return false;
        }
        if(data.pricePerUnit<0){
            setError("Lỗi data")
            return false;
        }
        return true;
    }
    const tao=async (data)=>{
        if(checkData(data)) {
            try {
                setLoading(true)
                const response = await fetch('https://localhost:7075/api/v1/Products', {
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
                navigate("/SanPham")
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
            <div className="container d-flex justify-content-center p-3 w-100">
                <form className="w-50 border border-3 rounded-4 p-3 bg-white d-flex justify-content-center flex-column"
                      onSubmit={handleSubmit(onSubmit)}>
                    <button className="btn btn-outline-dark border-3 fw-bold  text-start mb-2" style={{width: "120px"}}
                            type={"button"} onClick={() => navigate(-1)}><i className="bi bi-backspace"> Quay về</i>
                    </button>
                    <p className="h1 text-center mb-3">Tạo sản phẩm</p>
                    <div className="row">
                        <div className="col-4">
                            <div className="form-floating mb-3">
                                <input type="text" className="form-control rounded-0 border-2"
                                       id="floatingInput" {...register("name")} placeholder="Tên"/>
                                <label htmlFor="floatingInput">Tên</label>
                            </div>
                        </div>
                        <div className="col-4">
                            <div className="form-floating mb-3">
                                <input type="number" className="form-control rounded-0 border-2"
                                       id="floatingInput" {...register("pricePerUnit")} placeholder="Giá"/>
                                <label htmlFor="floatingInput">Giá(VNĐ)</label>
                            </div>
                        </div>
                        <div className="col-4">
                            <div className="form-floating mb-3">
                                <input type="text" className="form-control rounded-0 border-2"
                                       id="floatingInput" {...register("measureUnit")} placeholder="Đơn vị tính"/>
                                <label htmlFor="floatingInput">Đơn vị tính</label>
                            </div>
                        </div>
                        <div className="col-12">
                            <div className="form-floating mb-3">
                                <select className="form-select" id="floatingSelect"
                                        aria-label="Floating label select example" {...register("groupId")}>
                                    <option value="" selected>Không có nhóm</option>
                                    {dsGroup.map(group =>
                                        <option value={group.id}>{group.name}</option>
                                    )}
                                </select>
                                <label htmlFor="floatingSelect">Nhóm</label>
                            </div>
                        </div>
                    </div>
                    <h4 className="text-danger">
                        {error}
                    </h4>
                    <div className="d-flex flex-column justify-content-center w-100 pb-4">
                        <button
                            className={`btn btn-outline-success fw-bolder border-3 w-50 m-auto mt-2 rounded-0 ${loading ? "disabled" : ""}`}
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