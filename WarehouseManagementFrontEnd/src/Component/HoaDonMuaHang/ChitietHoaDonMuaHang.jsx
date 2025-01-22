import {useParams, useNavigate, Navigate, Link} from "react-router-dom";
import {useEffect, useState} from "react";
import {CompactTable} from "@table-library/react-table-library/compact";
import {useTheme} from "@table-library/react-table-library/theme";

export default function ChitietHoaDonMuaHang(props) {
    //
    const {id} = useParams()
    const [receipt, setReceipt] = useState({
        customer: {},
        Details: [],
        dateOrder: new Date()
    })
    const [productList, setProductList] = useState([]);
    const [showProductList, setShowProductList] = useState([]);
    const [customerList, setCustomerList] = useState([]);
    //
    const [deleteModal, setDeleteModal] = useState(false)
    const [editMode, setEditMode] = useState(false)
    const [editModel, setEditModel] = useState({})
    const [editError, setEditError] = useState("")
    const [addModel, setAddModel] = useState({
        product: "",
        quantity: 1,
    });
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState("")
    const navigate = useNavigate();

    const checkData = (data) => {
        if (nodes.length < 1 || data.customerId === "" || data.dateOrder === "") {
            setEditError("Chưa nhập đầy đủ thông tin")
            return false;
        }
        if (nodes.some(n => n.quantity <= 0)) {
            setEditError("Có sản phẩm chưa đủ số lượng")
            return false;
        }
        return true
    }
    const Edit = async () => {
        if (checkData(editModel)) {
            let list = []
            nodes.map(n => {
                list.push({productId: n.id, quantity: n.quantity})
            })
            const response = await fetch(`https://localhost:7075/api/v1/customer-receipts`, {
                headers: {'Content-Type': 'application/json'},
                credentials: 'include',
                body: JSON.stringify({
                    id: editModel.id,
                    customerId: editModel.customerId,
                    dateOrder: editModel.dateOrder,
                    details: list
                }),
                method: "PUT"
            });
            if (!response.ok) {
                const text = await response.text();
                setEditError(text);
            }
            setEditMode(false)
            await getReceipt();
        }
    }
    const getCustomerList = async () => {
        const response = await fetch('https://localhost:7075/api/v1/customers', {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "GET"
        });
        if (!response.ok) {
            const text = await response.text();
            throw Error(text);
        }
        const content = await response.json();
        setCustomerList(content);
    }
    const getProductList = async () => {
        const response = await fetch('https://localhost:7075/api/v1/Products', {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "GET"
        });
        if (!response.ok) {
            const text = await response.text();
            throw Error(text);
        }
        const content = await response.json();
        setProductList(content);
    }
    const getReceipt = async () => {
        setLoading(true);
        const response = await fetch(`https://localhost:7075/api/v1/customer-receipts/${id}`, {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "GET"
        });
        if (!response.ok) {
            navigate("/Error")
        }
        setLoading(false)
        const content = await response.json();
        setEditModel({})
        let list = []
        content.details.map(d => {
            list.push({
                id: d.productId,
                name: d.productName,
                pricePerUnit: d.totalPrice / d.quantity,
                quantity: d.quantity
            })
        })
        setEditModel({
            id: content.id,
            customerId: content.customer.id,
            dateOrder: new Date(content.dateOrder).getFullYear() + "-" + ("0" + (new Date(content.dateOrder).getMonth() + 1)).slice(-2) + "-" + ("0" + new Date(content.dateOrder).getDate()).slice(-2),
        })
        setNodes(list)
        setReceipt(content);
    }
    const Delete = async () => {
        const response = await fetch(`https://localhost:7075/api/v1/customer-receipts/${id}`, {
            headers: {'Content-Type': 'application/json'},
            credentials: 'include',
            method: "DELETE"
        });
        if (!response.ok) {
            navigate("/Error")
        }
        navigate("/HoaDonMuaHang");
    }
    const cancelEdit = () => {
        setEditMode(false)
        setEditModel({})
        let list = []
        receipt.details.map(d => {
            list.push({
                id: d.productId,
                name: d.productName,
                pricePerUnit: d.totalPrice / d.quantity,
                quantity: d.quantity
            })
        })
        setNodes(list)
        setEditModel({
            id: receipt.id,
            customerId: receipt.customer.id,
            dateOrder: new Date(receipt.dateOrder).getFullYear() + "-" + ("0" + (new Date(receipt.dateOrder).getMonth() + 1)).slice(-2) + "-" + ("0" + new Date(receipt.dateOrder).getDate()).slice(-2),
        })
    }


    //Table
    const theme = useTheme({
        HeaderRow: `
        .th {
          border: 1px solid black;
          border-bottom: 3px solid black;
           background-color: #51973FFF;
           text-align: center;
        }
      `,
        BaseCell: `
        
      `,
        Row: `
        .td {
          border: 1px solid black;
          
          background: linear-gradient(180deg, rgba(218,218,230,1) 0%, rgba(255,254,254,1) 99%);
        }

       
      `,
        Table: `
        --data-table-library_grid-template-columns:  1fr 1fr 1fr 1fr 1fr ;
      `,
    });
    const [nodes, setNodes] = useState([]);
    const COLUMNS = [
        {label: 'Tên', renderCell: (item) => item.name, resize: true},
        {label: 'Giá', renderCell: (item) => new Intl.NumberFormat().format(item.pricePerUnit) + " VNĐ", resize: true},
        {
            label: 'Số lượng',
            renderCell: (item) => editMode ?
                <input  className="form-control rounded-0 w-100" onChange={(e) => updateDetail(item.id, e)} type="number"
                       value={item.quantity}/> : item.quantity,
            resize: true
        },
        {
            label: 'Tổng giá trị',
            renderCell: (item) => new Intl.NumberFormat().format(item.quantity * item.pricePerUnit) + " VNĐ",
            resize: true
        },
        {
            label: '', renderCell: (item) =>
                editMode ?
                    <div className="d-flex justify-content-center">
                        <button className="btn btn-danger" onClick={() => deleteDetail(item.id)}>Xóa</button>
                    </div> : ""
            , resize: false
        },
    ];
    const data = {nodes};


    //State change
    const deleteDetail = (id) => {
        let list = nodes.filter(p => p.id !== id);
        setNodes(list)
        updateShowProductList(list)
    }
    const addDetail = () => {
        let list = [...nodes]
        if (!list.some(p => p.id === addModel.product)) {
            productList.map(p => {
                if (p.id === addModel.product) {
                    list.push({
                        id: p.id,
                        name: p.name,
                        pricePerUnit: p.pricePerUnit,
                        quantity: addModel.quantity,
                    })
                }
            })
            setNodes(list)
            updateShowProductList(list)
            setAddModel({
                product: "",
                quantity: 1,
            })
        } else
            setError("Đã có sản phẩm trong hóa đơn")
    }
    const updateDetail = (id, e) => {
        let list = []
        nodes.map(d => {
            if (d.id === id) {
                let detail = {...d};
                if (e.target.value >= 0)
                    detail.quantity = e.target.value;
                else
                    detail.quantity = 0
                list.push(detail);
            } else
                list.push(d);
        });
        setNodes(list)
    }
    const changeProduct = (e) => {
        setAddModel({
            product: e.target.value,
            quantity: addModel.quantity
        })
    }
    const changeQuantity = (e) => {
        setAddModel({
            product: addModel.product,
            quantity: e.target.value<0 ? 0:e.target.value
        })
    }
    const updateShowProductList = (l) => {
        let list = productList.filter(p =>
            !l.some(n => n.id === p.id)
        )
        setShowProductList(list)
    }
    const changeCustomerId = (e) => {
        setEditModel({
            id: editModel.id,
            customerId: e.target.value,
            dateOrder: editModel.dateOrder,
        })
    }
    const changeOrderDate = (e) => {
        setEditModel({
            id: editModel.id,
            customerId: editModel.customerId,
            dateOrder: e.target.value,
        })
    }

    function handleKeyDown(event) {
        if (event.key === 'Enter') {
            Edit(editModel)
        }
    }
    function handleKeyDown1(event) {
        if (event.key === 'Enter') {
            addDetail()
        }
    }
    //UseEffect
    useEffect(() => {
        getReceipt()
        getCustomerList()
        getProductList()
    }, []);
    useEffect(() => {
        updateShowProductList(nodes)
    }, [nodes]);
    if (!props.user.isLogged && props.user.userId === "") {
        return <Navigate to="/login"></Navigate>
    }
    return (
        <>
            <div className="container pt-1 m-auto">
                <h1 className="text-center">Thông tin hóa đơn mua hàng</h1>
                <button className="btn btn-outline-dark border-3 fw-bold  text-start mb-2" style={{width: "120px"}}
                        onClick={() => navigate(-1)}><i className="bi bi-backspace"> Quay về</i></button>
                <div className="pt-4">
                    {!loading ?
                        <>
                            <div
                                className="row row-gap-3 rounded-5 border border-5 border-black bg-white p-3 text-center">
                                <div className="col-4">
                                    <h2>ID:</h2>
                                    <p>{receipt.id}</p>
                                </div>
                                <div className="col-4">
                                    <h2>Ngày thanh toán</h2>
                                    {editMode ?
                                        <div className="form-floating ">
                                            <input onKeyDown={handleKeyDown} type={"date"} className="form-control rounded-4 border-3"
                                                   onChange={(e) => changeOrderDate(e)} id="floatingInput"
                                                   value={editModel.dateOrder} placeholder="Tên"/>
                                            <label htmlFor="floatingInput">Ngày thanh toán</label>
                                        </div>
                                        : <p>{new Date(receipt.dateOrder).toLocaleString('En-GB', {
                                            year: "numeric",
                                            month: "2-digit",
                                            day: "2-digit",
                                            hour12: false
                                        })}</p>}
                                </div>
                                <div className="col-4">
                                    <h2>Ngày tạo:</h2>
                                    <p>{new Date(receipt.createDate).toLocaleString('En-GB', {hour12: false})}</p>
                                </div>
                                <hr/>
                                <div className="col-6">
                                    <h2>Tên khách hàng</h2>
                                    <p>{receipt.customer.name}</p>
                                </div>
                                <div className="col-6">
                                    <h2>ID khách hàng</h2>
                                    <p>{receipt.customer.id}</p>
                                </div>
                                {editMode ?
                                    <div className="col-12">
                                        <div className="form-floating ">
                                            <select className="form-select rounded-4 border-3 pb-1" id="floatingSelect"
                                                    onChange={(e) => changeCustomerId(e)}
                                                    aria-label="Floating label select example"
                                                    value={editModel.customerId}>
                                                <option value="" selected disabled>Lựa chọn khách hàng</option>
                                                {customerList.map(customer =>
                                                    <option value={customer.id}>{customer.name} ({customer.id})</option>
                                                )}
                                            </select>
                                            <label htmlFor="floatingInput">Khách hàng</label>
                                        </div>

                                    </div>
                                    : ""}
                                <hr/>
                                <div className="col-12">
                                    <h2>Hóa đơn</h2>
                                    {editMode ?
                                        <div className="d-flex pt-3 pb-3 gap-5">
                                            <div className="form-floating w-100">
                                                <select className="form-select" id="floatingSelect"
                                                        onChange={changeProduct}
                                                        aria-label="Floating label select example"
                                                        value={addModel.product}>
                                                    <option value="" selected disabled>Lựa chọn sản phẩm</option>
                                                    {showProductList.map(product =>
                                                        <option
                                                            value={product.id}>{product.name} - {product.id} - {product.pricePerUnit} VNĐ</option>
                                                    )}
                                                </select>
                                                <label htmlFor="floatingSelect">Sản phẩm</label>
                                            </div>
                                            <div className="d-flex fl">
                                                <div className="form-floating ">
                                                    <input onKeyDown={handleKeyDown1} type={"number"} className="form-control rounded-0 border-2"
                                                           value={addModel.quantity} onChange={changeQuantity}
                                                           id="floatingInput"
                                                           placeholder="Quantity"/>
                                                    <label htmlFor="floatingInput">Số lượng</label>
                                                </div>
                                            </div>
                                            <div className="d-flex justify-content-center w-25">
                                                <button className="btn btn-success m-auto w-100"
                                                        onClick={() => addDetail()}>Thêm
                                                </button>
                                            </div>
                                        </div> :
                                        ""
                                    }
                                    <div>
                                        <CompactTable columns={COLUMNS} data={data} theme={theme}/>
                                        {nodes.length === 0 ?
                                            <p className="text-center">Không có sản phẩm </p>
                                            :
                                            <div className="d-flex justify-content-end">
                                                <h3>Tổng giá trị hóa
                                                    đơn: {new Intl.NumberFormat().format(nodes.reduce((acc, o) => acc + parseInt(o.pricePerUnit * o.quantity), 0))} VNĐ</h3>
                                            </div>
                                        }
                                    </div>

                                </div>
                            </div>


                            <h4 className="text-danger">{editError}</h4>

                            {
                                editMode ?
                                    <div className="d-flex flex-row gap-4 pb-5">
                                        <button className="btn btn-secondary" onClick={() => cancelEdit()}>Hủy
                                        </button>
                                        <button className="btn btn-success"
                                                onClick={() => Edit()}>Save
                                        </button>

                                    </div>
                                    :
                                    <div className="d-flex flex-row gap-4 pb-5">
                                        <button className="btn btn-danger" onClick={() => setDeleteModal(true)}>Xóa
                                        </button>
                                        <button className="btn btn-secondary" onClick={() => {
                                            setEditMode(true)
                                            getCustomerList();
                                        }}>Sửa
                                        </button>
                                    </div>
                            }
                            <div className={'modalpanel ' + (deleteModal ? "modal-active" : "")}>
                                <div
                                    className='modalpanel-content rounded-0  bg-white m-auto d-flex justify-content-between flex-column'>
                                    <div className='container-fluid d-flex justify-content-center'>
                                        <p className="h1">Xóa nhóm {receipt.id}</p>
                                    </div>
                                    <div className='modalpanel-content-text p-3'>
                                        Bạn có muốn xóa nhóm này?
                                    </div>
                                    <div className='align-bottom d-flex gap-3 justify-content-center p-2'>
                                        <button className='btn btn-secondary w-50'
                                                onClick={() => setDeleteModal(false)}>Hủy
                                        </button>
                                        <button className='btn btn-danger w-50' onClick={() => Delete()}>Ok</button>
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