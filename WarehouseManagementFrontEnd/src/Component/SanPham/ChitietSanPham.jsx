import {useParams, useNavigate, Navigate, Link} from "react-router-dom";
import {useEffect, useState} from "react";

export default function ChitietSanPham(props) {
    const {id} = useParams()
    const [dsGroup, setDsGroup] = useState([]);
    const [deleteModal, setDeleteModal] = useState(false)
    const [editMode, setEditMode] = useState(false)
    const [editModel, setEditModel] = useState({})
    const [editError, setEditError] = useState("")
    const [loading, setLoading] = useState(false)
    const navigate = useNavigate();
    const [err, setErr] = useState(false);
    const [product, setProduct] = useState({
        group:{}
    })
    const checkData=(data)=>{
        setEditError("")
        if(data.name.length===0||data.measureUnit.length===0||data.pricePerUnit.length===0){
            setEditError("Chưa nhập đầy đủ thông tin")
            return false;
        }
        if(data.pricePerUnit<0){
            setEditError("Lỗi data")
            return false;
        }
        return true;
    }
    const Edit = async (data) => {
       if(checkData(data)) {
           const response = await fetch(`https://localhost:7075/api/v1/Products`, {
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
           getProduct();
       }
    }
    const getDs = async () => {
        const response = await fetch('https://localhost:7075/api/v1/Product-groups', {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "GET"
        });
        if (!response.ok) {
            const text = await response.text();
            throw Error(text);
        }
        const content = await response.json();
        setDsGroup(content);
    }
    const getProduct = async () => {
        setLoading(true);
        const response = await fetch(`https://localhost:7075/api/v1/Products/${id}`, {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "GET"
        });
        if (!response.ok) {
            navigate("/Error")
        }
        setLoading(false)
        const content = await response.json();
        setEditModel({
            id: content.id,
            name: content.name,
            pricePerUnit:content.pricePerUnit,
            measureUnit: content.measureUnit,
            groupId: content.group!=null ? content.group.id : "",
        })
        setProduct(content);
    }
    const Xoa = async () => {
        const response = await fetch(`https://localhost:7075/api/v1/Products/${id}`, {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "DELETE"
        });
        if (!response.ok) {
            navigate("/Error")
        }
        navigate("/SanPham");
    }
    useEffect(() => {
        getProduct()
    }, []);
    const changeEditName = (e) => {
        setEditModel({
            id: editModel.id,
            name: e.target.value,
            pricePerUnit:editModel.pricePerUnit,
            measureUnit: editModel.measureUnit,
            groupId: editModel.groupId,
        })
    }
    const changeEditPrice = (e) => {
        setEditModel({
            id: editModel.id,
            name: editModel.name,
            pricePerUnit:e.target.value,
            measureUnit: editModel.measureUnit,
            groupId: editModel.groupId,
        })
    }
    const changeEditUnit = (e) => {
        setEditModel({
            id: editModel.id,
            name: editModel.name,
            pricePerUnit:editModel.pricePerUnit,
            measureUnit: e.target.value,
            groupId: editModel.groupId,
        })
    }
    const changeEditGroupId = (e) => {
        setEditModel({
            id: editModel.id,
            name: editModel.name,
            pricePerUnit:editModel.pricePerUnit,
            measureUnit: editModel.measureUnit,
            groupId: e.target.value,
        })
    }
    function handleKeyDown(event) {
        if (event.key === 'Enter') {
            Edit(editModel)
        }
    }
    if(!props.user.isLogged && props.user.userId===""){
        return <Navigate to="/login"></Navigate>
    }
    return (
        <>
            <div className="container pt-1 m-auto">
                <h1 className="text-center fw-bold">Thông tin sản phẩm</h1>
                <button className="btn btn-outline-dark border-3 fw-bold  text-start mb-2" style={{width: "120px"}}
                        onClick={() => navigate(-1)}><i className="bi bi-backspace"> Quay về</i></button>
                <div className="pt-4">
                    {!loading ?
                        <>
                            <div
                                className="row row-gap-3 rounded-5 border border-5 border-black bg-white p-3 text-center">
                                <div className="col-6">
                                    <h2>ID</h2>
                                    <p>{product.id}</p>
                                </div>
                                <div className="col-6">
                                    <h2>Ngày tạo</h2>
                                    <p>{new Date(product.createDate).toLocaleString('En-GB', {hour12: false})}</p>
                                </div>
                                <hr/>
                                <div className="col-4">
                                    <h2>Tên</h2>
                                    {editMode ?
                                        <div className="form-floating">
                                            <input onKeyDown={handleKeyDown} onChange={changeEditName} type="text" value={editModel.name}
                                                   className="form-control rounded-4 border-3  border-2"
                                                   id="floatingInput"
                                                   placeholder="Đơn vị tính"/>
                                            <label htmlFor="floatingInput">Tên sản phẩm</label>
                                        </div> : <p>{product.name}</p>}
                                </div>
                                <div className="col-4">
                                    <h2>Đơn vị tính</h2>
                                    {editMode ?
                                        <div className="form-floating">
                                            <input onKeyDown={handleKeyDown} onChange={changeEditUnit} type="text"
                                                   value={editModel.measureUnit}
                                                   className="form-control rounded-4 border-3  border-2"
                                                   id="floatingInput"
                                                   placeholder="Đơn vị tính"/>
                                            <label htmlFor="floatingInput">Đơn vị tính</label>
                                        </div> : <p>{product.measureUnit}</p>}
                                </div>
                                <div className="col-4">
                                    <h2>Giá</h2>
                                    {editMode ?
                                        <div className="form-floating">
                                            <input onKeyDown={handleKeyDown} onChange={changeEditPrice} type="number"
                                                   value={editModel.pricePerUnit}
                                                   className="form-control rounded-4 border-3  border-2"
                                                   id="floatingInput"
                                                   placeholder="Đơn vị tính"/>
                                            <label htmlFor="floatingInput">Giá(VNĐ)</label>
                                        </div> : <p>{new Intl.NumberFormat().format(product.pricePerUnit) + " VNĐ"}</p>}
                                </div>
                                <hr/>
                                {product.group !== null ?
                                    <>
                                        <div className="col-6">
                                            <h2>ID nhóm</h2>
                                            <p>{product.group.id}</p>
                                        </div>
                                        <div className="col-6">
                                            <h2>Tên nhóm</h2>
                                            <p>{product.group.name}</p>
                                        </div>
                                        {editMode ?
                                            <div className="col-12">
                                                <div className="form-floating">
                                                    <select className="form-select rounded-4 border-3 pb-1" id="floatingSelect"
                                                            aria-label="Floating label select example"
                                                            onChange={changeEditGroupId} value={editModel.groupId}>
                                                        <option value="" selected>Không có nhóm</option>
                                                        {dsGroup.map(group =>
                                                            <option value={group.id}>{group.name}({group.id})</option>
                                                        )}
                                                    </select>
                                                    <label htmlFor="floatingSelect">Nhóm</label>
                                                </div>
                                            </div>
                                            : ""}
                                    </> :
                                    <>
                                        <div className="col-12">
                                            <h2>Nhóm</h2>
                                            <p>Không có nhóm</p>
                                            {editMode ?
                                                <div className="form-floating">
                                                    <select className="form-select rounded-4 border-3" id="floatingSelect"
                                                            aria-label="Floating label select example"
                                                            onChange={changeEditGroupId} value={editModel.groupId}>
                                                        <option value="" selected>Không có nhóm</option>
                                                        {dsGroup.map(group =>
                                                            <option value={group.id}>{group.name}({group.id})</option>
                                                        )}
                                                    </select>
                                                    <label htmlFor="floatingSelect">Nhóm</label>
                                                </div>
                                                : ""}
                                        </div>
                                    </>}

                            </div>


                            <h4 className="text-danger">{editError}</h4>
                            {
                                editMode ?
                                    <div className="d-flex flex-row gap-4 pb-5">
                                        <button className="btn btn-secondary" onClick={() => {
                                            setEditMode(false)
                                            setEditModel({
                                                id: product.id,
                                                name: product.name,
                                                address: product.address,
                                                email: product.email,
                                                phoneNumber: product.phoneNumber,
                                                groupId: product.group!=null ? product.group.id : "",
                                            })
                                        }}>Hủy
                                        </button>
                                        <button type={"submit"} className="btn btn-success"
                                                onClick={() => Edit(editModel)}>Save
                                        </button>

                                    </div>
                                    :
                                    <div className="d-flex flex-row gap-4 pb-5">
                                        <button className="btn btn-danger" onClick={() => setDeleteModal(true)}>Xóa
                                        </button>
                                        <button className="btn btn-secondary" onClick={() => {
                                            setEditMode(true)
                                            getDs();
                                        }}>Sửa
                                        </button>
                                    </div>
                            }
                            <div className={'modalpanel ' + (deleteModal ? "modal-active" : "")}>
                                <div
                                    className='modalpanel-content rounded-4 border-3   bg-white m-auto d-flex justify-content-between flex-column'>
                                    <div className='container-fluid d-flex justify-content-center'>
                                        <p className="h1">Xóa nhóm {product.id}</p>
                                    </div>
                                    <div className='modalpanel-content-text p-3'>
                                        Bạn có muốn xóa nhóm này?
                                    </div>
                                    <div className='align-bottom d-flex gap-3 justify-content-center p-2'>
                                        <button className='btn btn-secondary w-50'
                                                onClick={() => setDeleteModal(false)}>Hủy
                                        </button>
                                        <button className='btn btn-danger w-50' onClick={() => Xoa()}>Ok</button>
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