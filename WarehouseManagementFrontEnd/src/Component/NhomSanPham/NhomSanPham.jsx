import {useState,useEffect} from "react";
import { CompactTable } from '@table-library/react-table-library/compact';
import {Link, Navigate} from "react-router-dom";
import {useTheme} from "@table-library/react-table-library/theme";
import {SortToggleType, useSort} from "@table-library/react-table-library/sort";
import {usePagination} from "@table-library/react-table-library/pagination";
export default function NhomSanPham(props){
    const [nodes,setNodes]=useState([]);
    const [err,setError]=useState("");
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
        setNodes(content);
    }

    useEffect(() => {
        getDs()
    }, []);


    const data = { nodes };
    const sort = useSort(
        data,
        {
            onChange: onSortChange,
        },
        {
            sortToggleType: SortToggleType.AlternateWithReset,
            sortFns: {
                id: (array) => array.sort((a, b) => a.id.localeCompare(b.id)),
                ten: (array) => array.sort((a, b) => a.name.localeCompare(b.name)),
                ngaytao: (array) => array.sort((a, b) => a.createDate - b.createDate),
                mota: (array) => array.sort((a, b) => a.description.localeCompare(b.description)),
            },
        }
    );
    function onSortChange(action, state) {

    }
    const theme = useTheme({
        HeaderRow: `
        .th {
          border: 1px solid black;
          border-bottom: 3px solid black;
           background-color: #eaf5fd;
        }
      `,
        BaseCell: `
        
      `,
        Row: `
        cursor: pointer;
        .td {
          border: 1px solid black;
          
          background-color: #9eb0f7;
        }

        &:hover .td {
          border-top: 1px solid orange;
          border-bottom: 1px solid orange;
        }
      `,
        Table: `
        --data-table-library_grid-template-columns:  1fr 1fr 1fr 1fr;
      `,
    });
    const COLUMNS = [
        { label: 'Id', renderCell: (item) => <Link className="link link-underline-opacity-0 fw-bolder" to={item.id}>{item.id} </Link>,sort: { sortKey: "id" }},
        { label: 'Tên', renderCell: (item) => item.name,sort: { sortKey: "ten" }},
        { label: 'Ngày tạo', renderCell: (item) => new Date(item.createDate).toLocaleString('En-GB', { hour12: false }),sort: { sortKey: "ngaytao" } },
        { label: 'Mô tả', renderCell: (item) => item.description==="" ? "Không có mô tả":item.description,sort: { sortKey: "mota" } },
    ];
    const pagination = usePagination(data, {
        state: {
            page: 0,
            size: 10,
        },
        onChange: onPaginationChange,
    });
    function onPaginationChange(action, state) {
    }
    if(!props.user.isLogged && props.user.userId===""){
        return <Navigate to="/login"></Navigate>
    }
    return (<>
        <div className="p-5 pt-0">
            <h1 className="pt-4 text-center">Danh sách nhóm sản phẩm</h1>
            <Link className="btn btn-outline-primary border-2 fw-bold mb-2" to="tao"><i className="bi bi-plus-circle"> Tạo thêm nhóm</i></Link>
            <CompactTable columns={COLUMNS} data={data} theme={theme} sort={sort}
                          layout={{custom: true, horizontalScroll: true}} pagination={pagination}/>
            {nodes.length === 0 ? <p className="text-center">Không có dữ liệu </p> :
                <div className="d-flex justify-content-end">
                       <span>
          Trang:{" "}
                           {pagination.state.getPages(data.nodes).map((_, index) => (
                               <button
                                   className={`btn ${pagination.state.page === index ? "btn-dark" : "btn-outline-dark"} btn-sm`}
                                   key={index}
                                   type="button"
                                   style={{
                                       marginRight: "5px",
                                       fontWeight: pagination.state.page === index ? "bold" : "normal",
                                   }}
                                   onClick={() => pagination.fns.onSetPage(index)}
                               >
                                   {index + 1}
                               </button>
                           ))}
        </span>
                </div>}
        </div>
    </>)
}