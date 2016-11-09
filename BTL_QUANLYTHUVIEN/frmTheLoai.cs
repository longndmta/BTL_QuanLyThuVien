﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;

namespace BTL_QUANLYTHUVIEN
{
    public partial class frmTheLoai : Form
    {
        public bool them = true;
        public bool sua = false;
        public int soLuongSV = 0;
        public bool daChuanhoa = false;
        public frmTheLoai()
        {
            InitializeComponent();
            // This line of code is generated by Data Source Configuration Wizard
            // Fill a SqlDataSource
        }

        private void btnDSTheLoai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Enable(true);
            Load();
        }
        public void Enable(bool logged)
        {
            btnCapNhap.Enabled = logged;
            btnLuu.Enabled = logged;
            btnHuy.Enabled = logged;
            btnThem.Enabled = logged;
            btnSua.Enabled = logged;
            btnXoa.Enabled = logged;

        }
        private void Load()
        {
            try
            {
                TLbindingSource.DataSource = TheLoai_BUS.DanhSachTL();
                gcTheLoai.DataSource = TLbindingSource;
            }
            catch
            {
                MessageBox.Show("Load bị lỗi!");
            }
        }

        private void gvTheLoai_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if(gvTheLoai.FocusedRowHandle>=0)
            {
                tbMaTL.Text = gvTheLoai.GetFocusedRowCellValue(colMaTL).ToString();
                tbTenTL.Text = gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString();
                sua = false;    
            }
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tbTenTL.Focus();
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(them)
            {
                tbTenTL.Clear();
                //mã thể loại tự tăng
                string maTL =nextMa(TheLoai_BUS.LastMaTL());
                them = false;
                TLbindingSource.AddNew();
                //GÁN vào textbox và gridview
                gvTheLoai.SetFocusedRowCellValue(colMaTL, maTL);
                tbMaTL.Text = maTL;
                //trỏ tới dòng cuối cùng trong gridview
                gvTheLoai.FocusedRowHandle = gvTheLoai.RowCount - 1;
                tbTenTL.Focus();
            }
            else
            {
                gvTheLoai.FocusedRowHandle = gvTheLoai.RowCount - 1;
                tbTenTL.Focus();
            }
        }
        //Hàm tự tăng mã THỂ loại
        public string nextMa(string s)
        {
            string sub1 = s.Substring(0, 2);
            string sub2 = s.Substring(2).Trim();
            int ma = Convert.ToInt32(sub2) + 1;
            string sub = "";
            //chèn thêm các kí tự 0 vào mã
            for (int i = 0; i < sub2.Length - ma.ToString().Length; i++)
                sub += "0";
            sub += ma.ToString();
            sub1 += sub;
            //
            return sub1;
        }
        //chuẩn hóa chuỗi
        public string ChuanHoa(string st)
        {
            string st1 = "";
            st = st.Trim();
            while (st.Length != 0)
            {
                st += " ";
                int i = st.IndexOf(' ');
                string s = char.ToUpper(st[0]) + st.Substring(1, i);
                st1 += s;
                st = st.Substring(i + 1).Trim();
            }
            return st1.Trim();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!them && gvTheLoai.FocusedRowHandle == gvTheLoai.RowCount - 1)
                {
                    if (tbTenTL.Text != "" )
                    {
                        //chuẩn hóa tên
                        string tenTL = ChuanHoa(tbTenTL.Text.ToLower());
                        TheLoai_BUS.themTL(tbMaTL.Text, tenTL);
                        MessageBox.Show("Thêm thành công");
                        // this.SinhVienbindingSource.EndEdit();
                        //thêm trực tiếp sinh viên vào gridview mà không load lại danh sách
                        gvTheLoai.SetFocusedRowCellValue(colTenTL, tenTL);
                        //load_DS();
                        //chuẩn hóa ở textbox
                        tbTenTL.Text = tenTL;
                        //
                        them = true;
                    }
                    else 
                    {
                        MessageBox.Show("Nhập tên thể loại");
                    }
                }
                else
                {
                    if (MessageBox.Show("Bạn có chắc muốn sửa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            if (ChuanHoa(tbTenTL.Text.Trim().ToLower()) != gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString().Trim())
                            {
                                string tenTL=ChuanHoa(tbTenTL.Text.ToLower());
                                TheLoai_BUS.suaTL(tbMaTL.Text.Trim(), tenTL);
                                gvTheLoai.SetFocusedRowCellValue(colTenTL, tenTL);
                                tbTenTL.Text=tenTL;
                            }
                  
                            MessageBox.Show("Sửa thành công!");
                            sua = true;
                        }
                        catch
                        {
                            MessageBox.Show("Sửa không thành công");
                        }
                    }
                    else
                    {
                        tbTenTL.Text=gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString();
                    }
                }

            }
            catch
            {
                MessageBox.Show("Lưu thất bại");
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                TheLoai_BUS.xoaTL(gvTheLoai.GetFocusedRowCellValue(colMaTL).ToString());
                TLbindingSource.RemoveAt(gvTheLoai.FocusedRowHandle);
                MessageBox.Show("Xóa thành công!");
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //hủy khi không muốn thêm thể loại
            // MessageBox.Show(gvSinhVien.FocusedRowHandle+" "+gvSinhVien.RowCount);
            if (!them && gvTheLoai.FocusedRowHandle == gvTheLoai.RowCount - 1)
            {
                them = true;
                TLbindingSource.RemoveAt(gvTheLoai.RowCount - 1);
            }
            //hủy khi sửa
            else
            {
                tbTenTL.Text = gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString();
            }
        }
        //khi sửa ở 1 dòng trên gridview và trỏ sang dòng khác thì có sự kiện 
        private void gvTheLoai_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            if (!them)
            {
                if (gvTheLoai.FocusedRowHandle != gvTheLoai.RowCount - 1)
                {

                }
                else
                {
                    if (MessageBox.Show("Bạn có chắc muốn lưu không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            if (gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString() != "")
                            {
                                //chuẩn hóa tên
                                string tenTL = ChuanHoa(gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString().ToLower());
                                string maTL = gvTheLoai.GetFocusedRowCellValue(colMaTL).ToString().Trim();
                                TheLoai_BUS.themTL(maTL, tenTL);
                                MessageBox.Show("Thêm thành công");
                                // this.SinhVienbindingSource.EndEdit();
                                //chuẩn hóa trực tiếp sinh viên vào gridview mà không load lại danh sách
                                gvTheLoai.SetFocusedRowCellValue(colTenTL, tenTL);
                                //load_DS();
                                //chuẩn hóa ở textbox
                                tbTenTL.Text = tenTL;
                                //
                                them = true;
                            }
                            else
                                MessageBox.Show("Nhập tên thể loại");

                        }
                        catch
                        {
                            MessageBox.Show("Lưu không thành công!");
                        }
                    }
                    else
                    {
                        them = true;
                        TLbindingSource.RemoveAt(gvTheLoai.RowCount - 1);
                    }
                }
            }
            else if (daChuanhoa)
            {
                daChuanhoa = false;
            }
            else if (sua)
            {
                sua = false;
            }
            else
            {
                string tenTL = ChuanHoa(gvTheLoai.GetFocusedRowCellValue(colTenTL).ToString().ToLower());
                string maTL=gvTheLoai.GetFocusedRowCellValue(colMaTL).ToString();
                //khi vừa mới thêm xong
                if (tbTenTL.Text==tenTL)
                {

                }
                else
                {
                    if (MessageBox.Show("Bạn có chắc muốn sửa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            // string maLop = Lop_BUS.maLop(cboLop.Text);
                            //  sv = new SinhVien_DTO(tbHoSV.Text, tbTenSV.Text, cboGioiTinh.Text, dtpNgaySinh.Value, tbDiaChi.Text, tbNoiSinh.Text, maLop);
                            //   SinhVien_BUS.suaSV(sv);
                            TheLoai_BUS.suaTL(maTL, tenTL);
                            gvTheLoai.SetFocusedRowCellValue(colTenTL, tenTL);
                            tbTenTL.Text = tenTL;
                            MessageBox.Show("Sửa thành công!");
                        }
                        catch
                        {
                            MessageBox.Show("Sửa không thành công!");
                        }
                    }
                    else
                    {
                        gvTheLoai.SetFocusedRowCellValue(colTenTL, tbTenTL.Text);
                    }
                }
            }
        }
        
    }
}