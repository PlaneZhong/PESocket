namespace PENet {
    using System;

    [Serializable]
    public abstract class PEMsg {
        public int seq;
        public int cmd;
        public int err;
    }
}