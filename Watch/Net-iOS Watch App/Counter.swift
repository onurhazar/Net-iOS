//
//  Counter.swift
//  Net-iOS Watch App
//
//  Created by Onur Hazar on 6/16/23.
//

import Combine
import WatchConnectivity

final class Counter: ObservableObject {
    var session: WCSession
    let delegate: WCSessionDelegate
    let subject = PassthroughSubject<Int, Never>()
    
    @Published private(set) var count: Int = 0
    
    init(session: WCSession = .default) {
        self.delegate = SessionDelegater(countSubject: subject)
        self.session = session
        self.session.delegate = self.delegate
        self.session.activate()
        
        subject
            .receive(on: DispatchQueue.main)
            .assign(to: &$count)
    }
    
    func increment() {
        count += 1
        session.sendMessage(["count": count], replyHandler: nil) { error in
            print(error.localizedDescription)
        }
    }
    
    func decrement() {
        if (count > 0) {
            count -= 1
            session.sendMessage(["count": count], replyHandler: nil) { error in
                print(error.localizedDescription)
            }
        }
    }
}
