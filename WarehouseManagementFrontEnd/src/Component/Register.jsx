import {useState} from "react";
import {useForm} from "react-hook-form";
import {Link, Navigate, useNavigate} from "react-router-dom";
import "../css/Register.css"

export default function Register(props) {
    const navigate = useNavigate()
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    const {register, handleSubmit, watch, formState: {errors}} = useForm();
    const onSubmit = data => Register(data);
    const checkData = (data) => {
        setError("")
        if (data.fullName.length === 0 || data.userName.length === 0 || data.password.length === 0 || data.confirmPassword.length === 0 || data.email.length === 0) {
            setError("Bạn chưa nhập thông tin đầy đủ!")
            return false;
        }
        if (data.password.length < 4) {
            setError("Độ dài mật khẩu phải phải lớn hơn 4!")
            return false
        }
        if (data.password !== data.confirmPassword) {
            setError("Mật khẩu không giống với mật khẩu xác nhận!")
            return false
        }
        return true;
    }

    async function Register(data) {
        if (checkData(data)) {
            try {
                setLoading(true)
                const response = await fetch('https://localhost:7075/api/Account/Register', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/json'},
                    credentials: 'include',
                    body: JSON.stringify(data)
                })
                if (!response.ok) {
                    const text = await response.text();
                    setError(text)
                }
                const content = await response.text();
                setSuccess(content)
            } catch (e) {
            } finally {
                setLoading(false)
            }
        }
    }

    if (props.user.isLogged) {
        return <Navigate to="/"></Navigate>
    }
    return (
        <>
            <div className="container d-flex justify-content-center h-100 ">
                <form className="m-auto w-50" onSubmit={handleSubmit(onSubmit)}>
                    <p className="h1 text-center mb-3">Đăng ký</p>
                    <div className="row">
                        <div className="col-6">
                            <div className="form-floating mb-3">
                                <input type="text" className="form-control  border-2"
                                       id="floatingInput" {...register("fullName")} placeholder="name"/>
                                <label htmlFor="floatingInput">Tên người dùng</label>
                            </div>
                        </div>
                        <div className="col-6">
                            <div className="form-floating mb-3">
                                <input type="text" className="form-control  border-2"
                                       id="floatingInput" {...register("userName")} placeholder="username"/>
                                <label htmlFor="floatingInput">Tên đăng nhập</label>
                            </div>
                        </div>
                        <div className="col-6">
                            <div className="form-floating mb-3">
                                <input type="password" className="form-control  border-2"
                                       id="floatingInput" {...register("password")} placeholder="password"/>
                                <label htmlFor="floatingInput">Mật khẩu</label>
                            </div>
                        </div>
                        <div className="col-6">
                            <div className="form-floating mb-3">
                                <input type="password" className="form-control  border-2"
                                       id="floatingInput" {...register("confirmPassword")} placeholder="confirmpassword"
                                />
                                <label htmlFor="floatingInput">Xác nhận mật khẩu</label>
                            </div>
                        </div>
                    </div>
                    <div className="d-flex flex-column">
                        <div className="form-floating mb-3">
                            <input type="email" className="form-control  border-2"
                                   id="floatingInput" {...register("email")} placeholder="email"/>
                            <label htmlFor="floatingInput">Email</label>
                            <div>
                            </div>
                        </div>
                    </div>
                    <h5 className="text-danger">{error}</h5>
                    <Link className="link mb-3" to="/Login">Đã có tài khoản?</Link>
                    <button
                        className={`btn btn-outline-success rounded-5 fw-bolder border-3 w-100 mt-2 mb-1  ${loading ? "disabled" : ""}`}
                        type="submit">{loading ?
                        <>
                            <span className="spinner-grow spinner-grow-sm ms-1" role="status" aria-hidden="true"></span>
                            <span className="spinner-grow spinner-grow-sm ms-1" role="status" aria-hidden="true"></span>
                            <span className="spinner-grow spinner-grow-sm ms-1" role="status" aria-hidden="true"></span>
                        </> : <>
                            Đăng ký
                        </>
                    }</button>
                </form>
            </div>
            <div className={'modalpanel ' + (success !== "" ? "modal-active" : "")}>
                <div className='modalpanel-content   bg-success rounded-0 m-auto d-flex justify-content-between flex-column'>
                    <div className='container-fluid d-flex justify-content-center'>
                        <p className="h1">Thành công</p>
                    </div>
                    <div className='modalpanel-content-text p-3'>
                        {success}
                    </div>
                    <div className='align-bottom d-flex gap-3 justify-content-center p-2'>
                        <button className='btn btn-dark w-50' onClick={() => navigate("/Login")}>Ok</button>
                    </div>
                </div>
            </div>
        </>
    )
}