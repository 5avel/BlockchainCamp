﻿using BlockchainCampTask.Models;
using BlockchainNode.DAL;
using BlockchainNode.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BlockchainCampTask
{
    public class Blockchain
    {
        private BlockchainRepository blockchainRepository;
        private List<Transaction> _listTransaction = new List<Transaction>();

        public bool AddTransaction(Transaction transaction)
        {
            _listTransaction.Add(transaction);
            if(_listTransaction.Count == Block.RowsCount)
                CreateNewBlock();
            return true;
        }

        private Status _curStatus;
        public Status CurStatus
        {
            get
            {
                if(_curStatus == null)
                {
                    _curStatus = blockchainRepository.GetStatus();
                }
                return _curStatus;
            }
        }

        public bool Sync()
        {
            Neighbour neighbour = CurStatus.neighbours.FirstOrDefault();

            if (neighbour == null) return false;

            using (WebClient client = new WebClient())
            {
                string json =  client.DownloadString(neighbour.url + "/blockchain/get_blocks/10000");
                List<Block> listBlocks = JsonConvert.DeserializeObject<List<Block>>(json);

                listBlocks.Reverse();

                if(listBlocks == null || listBlocks.Count() == 0) return false;

                blockchainRepository.DelAllBlock();
                foreach (Block item in listBlocks)
                {
                    blockchainRepository.AddNewBlock(item);
                    CurStatus.last_hash = item.hash;
                }

                UpdateStatus();
                
                return true;
            }
        }

        internal bool AddNewNeighbour(Neighbour neighbour)
        {
            foreach (Neighbour n in CurStatus.neighbours)
            {
                if (neighbour.id == n.id) return false;
            }
            CurStatus.neighbours.Add(neighbour);
           
            UpdateStatus();
            return true;
        }

        private void UpdateStatus()
        {
            blockchainRepository.UpdateStatus(_curStatus);
        }

        public IEnumerable<Block> GetLastBlocks(int blocksCount = 0)
        {
            List<Block> blocks = new List<Block>();
            if (CurStatus.last_hash == "0") return blocks; // блоков нет

            Block curBlock = blockchainRepository.GetBlockByHash(CurStatus.last_hash);
            blocks.Add(curBlock); // последний блок
            int counter = 0;
            while (curBlock.prev_hash != "0" )
            {
                curBlock = blockchainRepository.GetBlockByHash(curBlock.prev_hash);
                blocks.Add(curBlock);
                counter++;
                if (blocksCount > 0 && counter == blocksCount) break;
            }
            return blocks;
        }

        public bool AddNewBlosk(Block block, string senderId)
        {
            if (block.prev_hash == _curStatus.last_hash)
            {
                blockchainRepository.AddNewBlock(block);
                CurStatus.last_hash = block.hash;
                UpdateStatus();

                SendBlockToNeighbour(block, senderId); // отправить соседям
                return true;
            }
            else if(block.hash == _curStatus.last_hash)
            {
                Block myLastBlock = blockchainRepository.GetBlockByHash(_curStatus.last_hash);
                if (block.prev_hash == myLastBlock.prev_hash)
                {
                    if (block.ts < myLastBlock.ts)
                    {
                        blockchainRepository.DelBlockByHash(_curStatus.last_hash); // del my last block
                        blockchainRepository.AddNewBlock(block);
                        CurStatus.last_hash = block.hash;
                        UpdateStatus();
                        SendBlockToNeighbour(block, senderId); // отправить соседям
                        return true;
                    }
                }
            }
            return false;
        }

        private void SendBlockToNeighbour(Block blockToSend, string senderId = "")
        {
            List<Neighbour> neighbours = CurStatus.neighbours;
            if (neighbours == null || neighbours.Count() == 0) return;

            foreach (var item in neighbours)
            {
                if (item.id == senderId) continue; // пропуск сендера
                string respResult = NodeCommunication.SendObjectAsJson(
                    new { sender_id = CurStatus.id, block = blockToSend },
                    item.url+ "/blockchain/receive_update");
                try
                {
                    Answer answer = JsonConvert.DeserializeObject<Answer>(respResult);
                    //if (!answer.success) CurStatus.neighbours.Remove(item);
                    UpdateStatus();
                    Console.WriteLine(respResult);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }
        }


        private void CreateNewBlock()
        {
            Block block = new Block(CurStatus.last_hash, _listTransaction);
            blockchainRepository.AddNewBlock(block); 
            CurStatus.last_hash = block.hash;
            UpdateStatus();
            _listTransaction.Clear();
            SendBlockToNeighbour(block);
        }

        private Blockchain()
        {
            blockchainRepository = new BlockchainRepository();
        }
        private static Blockchain instance;
        public static Blockchain Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Blockchain();
                }
                return instance;
            }
        }

        
    }
}
