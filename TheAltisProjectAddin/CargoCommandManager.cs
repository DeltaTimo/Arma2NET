﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAltisProjectDatabase;

namespace TheAltisProjectAddin
{
    class CargoCommandManager
    {
        private IDatabaseCargo _IDatabaseCargo = null;
        private Result _Result = null;

        public CargoCommandManager()
        {
            _IDatabaseCargo = new DatabaseCargoSQLite(new LogManager(), System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DatabaseCargo.sqlite"));
        }

        private string SplitToString(string[] split)
        {
            if (split == null)
                return "split==null";
            if (split.Length == 0)
                return "split.Length==0";

            string result = "";
            for (int i = 0; i < split.Length; i++)
            {
                result += i.ToString() + "(";
                if (split[i] != null)
                    result += split[i] + ") ";
                else
                    result += "null) ";
            }

            return result;
        }
        
        public string Parse(string[] split)
        {
            try
            {
                if (split.Length < 2)
                    return "ERROR_CARGO_SPLIT_LENGTH";

                if (split[1].ToLower() == "select")
                {
                    #region cargo|select|table|boxid|type
                    try
                    {
                        if (split.Length < 5)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_SELECT_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR_CARGO_SELECT_SPLIT_LENGTH";
                        }
                        if (_Result != null)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_SELECT_ACTIVE: Split=" + SplitToString(split));
                            return "ERROR_CARGO_SELECT_ACTIVE";
                        }

                        _Result = _IDatabaseCargo.Select(split[2], split[3], split[4]);

                        return "OK";
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: CARGO.Select failed: " + ex.Message);
                        return "ERROR_CARGO_SELECT_EXCEPTION";
                    }
                    #endregion
                }
                else if (split[1].ToLower() == "selectids")
                {
                    #region cargo|select|table
                    try
                    {
                        if (split.Length < 3)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_SELECTIDS_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR_CARGO_SELECTIDS_SPLIT_LENGTH";
                        }
                        if (_Result != null)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_SELECTIDS_ACTIVE: Split=" + SplitToString(split));
                            return "ERROR_CARGO_SELECTIDS_ACTIVE";
                        }

                        _Result = _IDatabaseCargo.SelectIds(split[2]);

                        return "OK";
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: CARGO.SelectIds failed: " + ex.Message);
                        return "ERROR_CARGO_SELECTIDS_EXCEPTION";
                    }
                    #endregion
                }
                else if (split[1].ToLower() == "selectnext")
                {
                    #region cargo|selectnext
                    try
                    {
                        if (split.Length < 2)
                        {
                            Arma2Net.Utils.Log("ERROR: CARGO.Selectnext ERROR_CARGO_SELECTNEXT_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR"; // Nur über ERROR beenden, sonst kann die SQF sich nicht beenden.
                        }

                        if (_Result == null)
                        {
                            Arma2Net.Utils.Log("ERROR: CARGO.Selectnext ERROR_CARGO_SELECTNEXT_INACTIVE: Split=" + SplitToString(split));
                            return "ERROR"; // Nur über ERROR beenden, sonst kann die SQF sich nicht beenden.
                        }

                        string result = _Result.Next();
                        if (result == null)
                        {
                            _Result = null;
                            return "EOF"; // Alles gut, das war der letzt Eintrag
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: CARGO.Selectnext failed: " + ex.Message);
                        return "ERROR"; // Nur über ERROR beenden, sonst kann die SQF sich nicht beenden.
                    }
                    #endregion
                }
                else if (split[1].ToLower() == "insert")
                {
                    #region cargo|insert|table|boxid|type|data
                    try
                    {
                        if (split.Length < 6)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_INSERT_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR_CARGO_INSERT_SPLIT_LENGTH";
                        }
                        if (!_IDatabaseCargo.Insert(split[2], split[3], split[4], split[5]))
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_INSERT: Split=" + SplitToString(split));
                            return "ERROR_CARGO_INSERT";
                        }
                        
                        return "OK";
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: CARGO.Insert failed: " + ex.Message);
                        return "ERROR_CARGO_INSERT_EXCEPTION";
                    }
                    #endregion
                }
                else if (split[1].ToLower() == "deleteall")
                {
                    #region cargo|deleteall|table|boxid
                    try
                    {
                        if (split.Length < 4)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_DELETEALL_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR_CARGO_DELETEALL_SPLIT_LENGTH";
                        }
                        if (!_IDatabaseCargo.DeleteCargoId(split[2], split[3]))
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_DELETEALL: Split=" + SplitToString(split));
                            return "ERROR_CARGO_DELETEALL";
                        }
                        return "OK";
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: CARGO.DeleteAll failed: " + ex.Message);
                        return "ERROR_CARGO_DELETEALL_EXCEPTION";
                    }
                    #endregion
                }
                else if (split[1].ToLower() == "deletetype")
                {
                    #region cargo|deletetype|table|boxid|type
                    try
                    {
                        if (split.Length < 5)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_DELETETYPE_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR_CARGO_DELETETYPE_SPLIT_LENGTH";
                        }
                        if (!_IDatabaseCargo.DeleteCargoType(split[2], split[3], split[4]))
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_DELETETYPE: Split=" + SplitToString(split));
                            return "ERROR_CARGO_DELETETYPE";
                        }

                        return "OK";
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: CARGO.DeleteType failed: " + ex.Message);
                        return "ERROR_CARGO_DELETETYPE_EXCEPTION";
                    }
                    #endregion
                }
                else if (split[1].ToLower() == "init")
                {
                    #region cargo|init|table
                    try
                    {
                        if (split.Length < 3)
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_INIT_SPLIT_LENGTH: Split=" + SplitToString(split));
                            return "ERROR_CARGO_INIT_SPLIT_LENGTH";
                        }
                        if (!_IDatabaseCargo.OpenOrCreateTable(split[2]))
                        {
                            Arma2Net.Utils.Log("ERROR_CARGO_INIT: Split=" + SplitToString(split));
                            return "ERROR_CARGO_INIT";
                        }

                        return "OK";
                    }
                    catch (Exception ex)
                    {
                        Arma2Net.Utils.Log("ERROR: Cargo.Init failed: " + ex.Message);
                        return "ERROR_CARGO_INIT_EXCEPTION";
                    }
                    #endregion
                }
                else
                    return "ERROR_CARGO_INVALID_COMMAND";
            }
            catch (Exception ex)
            {
                Arma2Net.Utils.Log("ERROR: CARGO-Exception: " + ex.Message);
                return "ERROR_CARGO_EXCEPTION";
            }
        }
    }
}
