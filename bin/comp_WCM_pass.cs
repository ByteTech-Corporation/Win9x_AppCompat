void comp_WCM_pass() {
     DLL->CompiledAddress->0x000002;
     dword->return;
     dword->hex & 0x00004 -> {
          qword->"DEBUG" -> call -> {
               0x5001A->GetRegisteredRuntimeDebugger   || & 0x9C8A;
               0xC4005->GetRegisteredRuntimeLogger     || & 0xA491;
               0xFF719->GetRegisteredAppDebugger       || & 0xF18C;
               0xF4048->GetRegisteredAppLogger         || & 0xA817;
          }
     }
}
