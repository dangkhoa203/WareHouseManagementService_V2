import {useParams, useNavigate, Navigate, Link} from "react-router-dom";
import {useEffect, useState} from "react";

export default function ChitietNhomNhaCungCap(props) {
    const {id} = useParams()
    const [deleteModal, setDeleteModal] = useState(false)
    const [editMode, setEditMode] = useState(false)
    const [editModel, setEditModel] = useState({})
    const [editError, setEditError] = useState("")
    const [loading, setLoading] = useState(false)
    const navigate = useNavigate();
    const [err, setErr] = useState(false);
    const [group, setGroup] = useState({})
    const checkData = (data) => {
        setEditError("");
        if (data.name.length === 0) {
            setEditError("Bạn chưa nhập đủ thông tin!")
            return false;
        }
        return true;
    }
    const Edit = async (data) => {
        if (checkData(data)) {
            const response = await fetch(`https://localhost:7075/api/v1/Vendor-Groups`, {
                headers: {'Content-Type': 'application/json'},
                credentials: 'include',
                body: JSON.stringify(data),
                method: "PUT"
            });
            if (!response.ok) {
                const text = await response.text();
                setEditError(text);
            }
            setEditMode(false)
            getDs();
        }
    }
    const getDs = async () => {
        setLoading(true);
        const response = await fetch(`https://localhost:7075/api/v1/Vendor-Groups/${id}`, {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "GET"
        });
        if (!response.ok) {
            navigate("/Error")
        }
        setLoading(false)
        const content = await response.json();
        setEditModel(content)
        setGroup(content);
    }
    const xoagroup = async () => {
        const response = await fetch(`https://localhost:7075/api/v1/Vendor-Groups/${id}`, {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "DELETE"
        });
        if (!response.ok) {
            navigate("/Error")
        }
        navigate("/NhomNhaCungCap");
    }
    useEffect(() => {
        getDs()
    }, []);
    const changeEditName = (e) => {
        setEditModel({
            id: editModel.id,
            name:e.target.value,
            description:editModel.description,
        })
    }
    const changeEditDescription = (e) => {
        setEditModel({
            id: editModel.id,
            name:editModel.name,
            description:e.target.value,
        })
    }
    function handleKeyDown(event) {
        if (event.key === 'Enter') {
            Edit(editModel)
        }
    }
    if (group == null) {
        return <Navigate to="/Error"/>
    }
    if(!props.user.isLogged && props.user.userId===""){
        return <Navigate to="/login"></Navigate>
    }
    return (
        <>
            <div className="container pt-1 m-auto">
                <h1 className="text-center fw-bold">Chi tiết nhóm nhà cung cấp</h1>
                <button className="btn btn-outline-dark border-3 fw-bold  text-start mb-2" style={{width: "120px"}}
                        onClick={() => navigate(-1)}><i className="bi bi-backspace"> Quay về</i></button>
                <div className="pt-4">
                    {!loading ?
                        <>
                            <div className="row rounded-5 border border-5 border-black bg-white p-3 text-center">
                                <div className="col-4">
                                    <h2>ID</h2>
                                    <p>{group.id}</p>
                                </div>
                                <div className="col-4">
                                    <h2>Tên</h2>
                                    {editMode ?
                                        <div className="form-floating mb-3">
                                            <input onKeyDown={handleKeyDown} type="text" className="form-control rounded-4 border-3"
                                                   id="floatingInput" onChange={changeEditName} value={editModel.name}
                                                   placeholder="Tên"/>
                                            <label htmlFor="floatingInput">Tên</label>
                                        </div> :  <p>{group.name}</p>}
                                </div>
                                <div className="col-4">
                                    <h2>Ngày tạo</h2>
                                    <p>{new Date(group.createDate).toLocaleString('En-GB', {hour12: false})}</p>
                                </div>
                                <hr className="mt-2 mb-2"/>
                                <div className="col-12">
                                    <h2>Mô tả</h2>
                                    {editMode ?
                                        <div className="form-floating">
                            <textarea onKeyDown={handleKeyDown} className="form-control rounded-1 border-3 "
                                      id="floatingInput" onChange={changeEditDescription} value={editModel.description}
                                      placeholder="Mô tả"/>
                                            <label htmlFor="floatingInput">Mô tả</label>
                                        </div> : <p className="text-start p-2 m-0">{group.description==="" ? "Không có mô tả":group.description}</p>}
                                </div>
                            </div>
                            <h4 className="text-danger">{editError}</h4>
                            {
                                editMode ?
                                    <div className="d-flex flex-row gap-4 pb-5">
                                        <button className="btn btn-outline-secondary w-25 fw-bold border-3"
                                                onClick={() => setEditMode(false)}>Hủy
                                        </button>
                                        <button type={"submit"}
                                                className="btn btn-outline-success w-25 fw-bold border-3"
                                                onClick={() => Edit(editModel)}>Cập nhật
                                        </button>

                                    </div>
                                    :
                                    <div className="d-flex flex-row gap-4 pb-5">
                                        <button className="btn btn-outline-danger w-25 fw-bold border-3"
                                                onClick={() => setDeleteModal(true)}>Xóa
                                        </button>
                                        <button className="btn btn-outline-secondary w-25 fw-bold border-3"
                                                onClick={() => setEditMode(true)}>Sửa
                                        </button>
                                    </div>
                            }
                            <div className={'modalpanel ' + (deleteModal ? "modal-active" : "")}>
                                <div
                                    className='modalpanel-content rounded-4  bg-danger text-black m-auto d-flex justify-content-between flex-column'>
                                    <div className='container-fluid d-flex justify-content-center'>
                                        <p className="h1">Xóa nhóm {group.name}</p>
                                    </div>
                                    <div className='modalpanel-content-text p-3'>
                                        Bạn có muốn xóa nhóm({group.id}) này?
                                    </div>
                                    <div className='align-bottom d-flex gap-3 justify-content-center p-2'>
                                        <button className='btn btn-secondary w-50 fw-bold border-3'
                                                onClick={() => setDeleteModal(false)}>Hủy
                                        </button>
                                        <button className='btn btn-success w-50 fw-bold border-3'
                                                onClick={() => xoagroup()}>Ok
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </> :
                        <div className='text-center mt-4'>
                            <div className="spinner-border" role="status">
                                <span className="visually-hidden">Loading...</span>
                            </div>
                        </div>
                    }
                </div>
            </div>
        </>
    )
}