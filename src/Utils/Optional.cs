namespace Utils;

//    Might need an optional time later, this doesn't compile btw
//
//    public class Optional<T> {
//        private T? value;
//        public bool HasValue {get; private set;}
//
//        public Optional(T value) {
//            this.value = value;
//            HasValue = true;
//        }
//
//        private static Optional<T>? emptyOptional;
//        protected Optional() {
//            HasValue = false;
//        }
//
//        public static Optional<T> Empty() {
//            if(emptyOptional == null) {
//                emptyOptional = Optional();
//            }
//
//            return emptyOptional;
//        }
//
//        public T Value() {
//            if(!HasValue) throw new Exception("Called value when optional is empty");
//            return this.value;
//        }
//    }